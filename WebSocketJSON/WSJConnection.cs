using Dynamitey;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WebSocket4Net;
using KIARA;
using System.Runtime.Serialization.Formatters;

namespace WebSocketJSON
{
    public delegate object GenericWrapper(params object[] arguments);

    // TODO: rewrite plugin to use processing loop. This will allow to simplify code by removing locks in this class.
    public class WSJConnection : Connection
    {
        public WSJConnection(ISocket aSocket)
            : this()
        {
            socket = aSocket;
            socket.Closed += HandleClosed;
            socket.Message += HandleMessage;
        }

        public override event EventHandler Closed;

        public override void Disconnect()
        {
            socket.Close();
        }

        /// <summary>
        /// Handles an incoming message.
        /// </summary>
        /// <param name="message">The incoming message.</param>
        public void HandleMessage(object sender, MessageEventArgs e)
        {
            IMessage receivedMessage = null;
            // FIXME: Occasionally we receive JSON with some random bytes appended. The reason is
            // unclear, but to be safe we ignore messages that have parsing errors.
            try
            {
                data = Protocol.DeserializeMessage(e.Message);
            }
            catch (JsonException)
            {
                return;
            }

            MessageType msgType = receivedMessage.Type;
            if (msgType == MessageType.RESPONSE)
                HandleCallResponse(receivedMessage);
            else if (msgType == MessageType.EXCEPTION)
                HandleCallError(receivedMessage);
            else if (msgType == MessageType.REQUEST)
                HandleCall(receivedMessage);
            else
                SendCallError(-1, "Unknown message type: " + msgType);
        }

        protected override void ProcessIDL(string parsedIDL)
        {
            // TODO
        }

        protected override IClientFunctionCall CallClientFunction(string funcName, params object[] args)
        {
            int callID = getValidCallID();

            // Register delegates as callbacks. Pass their registered names instead.
            List<int> callbacks;
            List<object> convertedArgs = convertCallbackArguments(args, out callbacks);
            List<object> callMessage = createCallMessage(callID, funcName, callbacks, convertedArgs);

            FuncCallBase callObj = null;
            if (!IsOneWay(funcName))
            {
                string[] serviceDescription = funcName.Split('.');
                // Usually, a function called via CallClientFunction is parsed from the KIARA IDL and of ther
                // form serviceName.functionName. However, in some cases (e.g. twisted Unit Tests), functions may be
                // created locally, only having a GUID as function name. In this case, we appen "LOCAL" as service name
                // to mark the function as locally created
                if (serviceDescription.Length < 2)
                {
                    callObj = wsjFuncCallFactory.Construct("LOCAL", funcName);
                }
                else
                {
                    callObj = wsjFuncCallFactory.Construct(serviceDescription[0], serviceDescription[1]);
                }

                // It is important to add an active call to the list before sending it, otherwise we may end up
                // receiving call-reply before this happens, which will trigger unnecessary call-error and crash the
                // other end.
                lock (activeCalls)
                    activeCalls.Add(callID, callObj);
            }

            SendMessage(callMessage);

            return callObj;
        }

        protected override void RegisterHandler(string funcName, Delegate handler)
        {
            lock (registeredFunctions)
                registeredFunctions[funcName] = handler;
        }

        public override bool GetProperty(string name, out object value)
        {
            if (name == "JsonSerializerSettings")
            {
                value = settings;
                return true;
            }

            return base.GetProperty(name, out value);
        }

        public override bool SetProperty(string name, object value)
        {
            if (name == "JsonSerializerSettings" && value is JsonSerializerSettings)
            {
                settings = value as JsonSerializerSettings;
                serializer = JsonSerializer.Create(settings);
                return true;
            }

            return base.SetProperty(name, value);
        }

        internal WSJConnection()
        {
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.FloatParseHandling = FloatParseHandling.Double;
            settings.Converters.Add(new StandardFloatConverter());
            settings.ContractResolver = new PrivateSetterResolver();
            serializer = JsonSerializer.Create(settings);
        }

        internal void HandleClosed(object sender, EventArgs e)
        {
            FuncCallBase[] removedCalls = new FuncCallBase[activeCalls.Count];
            lock (activeCalls)
            {
                activeCalls.Values.CopyTo(removedCalls, 0);
                activeCalls.Clear();
            }

            foreach (var call in removedCalls)
                call.HandleError("Connection closed.");

            if (Closed != null)
                Closed(this, e);
        }

        internal virtual void SendSerializedMessage(string serializedMessage)
        {
            socket.Send(serializedMessage);
        }

        internal void SendMessage(List<object> message)
        {
            string serializedMessage = JsonConvert.SerializeObject(message, settings);
            SendSerializedMessage(serializedMessage);
        }

        private void HandleCall(IMessage callMessage)
        {
            int callID = callMessage.ID;
            string methodName = callMessage.MethodName;
            string[] serviceDescription = methodName.Split('.');

            Delegate nativeMethod = null;
            lock (registeredFunctions)
            {
                if (registeredFunctions.ContainsKey(methodName))
                    nativeMethod = registeredFunctions[methodName];
            }

            if (nativeMethod != null)
            {
                object[] parameters;
                try
                {
                    var args = callMessage.Parameters;
                    var callbacks = callMessage.Callbacks;
                    var paramInfo = new List<ParameterInfo>(nativeMethod.Method.GetParameters());
                    parameters = ConvertParameters(methodName, args, callbacks, paramInfo);
                }
                catch (Exception e)
                {
                    SendCallError(callID, e.Message);
                    return;
                }

                object returnValue = null;
                object exception = null;
                bool success = true;
                try
                {
                    // Super Evil Hack Here! Existing unit tests assume that WSJON serializes in a fixed format that
                    // originates from serializing the native types correctly. Also, the tests do not take into account
                    // any KTD from any IDL. To make them work, we have to pretend that there is no ServiceRegistry
                    // maintaining any service description, but bypass type check and automatic KTD Conversion
                    // by setting service Registry to null
                    if (ServiceRegistry.Instance == null)
                    {
                        returnValue = nativeMethod.DynamicInvoke(parameters);
                    }
                    else
                    {
                        ServiceFunctionDescription service = ServiceRegistry.Instance
                            .GetService(serviceDescription[0])
                            .GetServiceFunction(serviceDescription[1]);
                        returnValue = service.ReturnType.AssignValuesFromObject(nativeMethod.DynamicInvoke(parameters));
                    }
                }
                catch (Exception e)
                {
                    exception = e;
                    success = false;
                }

                if (!IsOneWay(methodName))
                    SendCallReply(callID, nativeMethod, success, returnValue, exception);
            }
            else
            {
                SendCallError(callID, "Method " + methodName + " is not registered");
                return;
            }
        }

        private object[] ConvertParameters(string methodName, List<object> args, List<int> callbacks, List<ParameterInfo> paramInfo)
        {
            object[] parameters = new object[paramInfo.Count];

            // Special handling for the first parameter if it's of type Connection.
            if (paramInfo.Count > 0 && paramInfo[0].ParameterType.Equals(typeof(Connection)))
            {
                parameters[0] = this;
                var otherParams = ConvertParameters(methodName, args, callbacks, paramInfo.GetRange(1, paramInfo.Count - 1));
                otherParams.CopyTo(parameters, 1);
                return parameters;
            }

            if (paramInfo.Count != args.Count)
            {
                throw new InvalidNumberOfArgs("Incorrect number of arguments for a method. Expected: " +
                                              paramInfo.Count + ". Received: " + args.Count);
            }

            for (int i = 0; i < args.Count; i++)
            {
                if (callbacks.Contains(i))
                {
                    if (paramInfo[i].ParameterType == typeof(ClientFunction))
                    {
                        parameters[i] = CreateFuncWrapperDelegate((string)args[i]);
                    }
                    else if (typeof(Delegate).IsAssignableFrom(paramInfo[i].ParameterType))
                    {
                        parameters[i] = CreateCustomDelegate((string)args[i], paramInfo[i].ParameterType);
                    }
                    else
                    {
                        throw new Exception("Parameter " + i + " is neither a delegate nor a FuncWrapper. " +
                                            "Cannot pass callback method in its place");
                    }
                }
                else
                {
                    // Super Evil Hack! See other super evil hack comment above
                    if (ServiceRegistry.Instance == null)
                    {
                        parameters[i] = Convert.ChangeType(args[i], paramInfo[i].ParameterType);
                    }
                    else
                    {
                        IDictionary<string, object> c = (Dictionary<string, object>)args[i];
                        string[] service = methodName.Split('.');
                        KtdType idlParameter = ServiceRegistry.Instance.GetService(service[0])
                            .GetServiceFunction(service[1]).Parameters.ElementAt(i).Value;
                        parameters[i] = idlParameter.AssignValuesToNativeType(c, paramInfo[i].ParameterType);
                    }

                }
            }

            return parameters;
        }

        private object CreateCustomDelegate(string funcName, Type delegateType)
        {
            Type retType = delegateType.GetMethod("Invoke").ReturnType;
            var genericWrapper = new GenericWrapper(arguments =>
            {
                if (retType == typeof(void))
                {
                    CallClientFunction(funcName, arguments);
                    // We do not wait here since SuperWebSocket doesn't process messages while the
                    // current thread is blocked. Waiting would bring the current client's thread
                    // into a deadlock.
                    return null;
                }
                else
                {
                    throw new NotImplementedException("We do not support callbacks with return " +
                        "value yet. This is because we cannot wait for a callback to complete. " +
                        "See more details here: https://redmine.viscenter.de/issues/1406.");

                    //object result = null;
                    //CallFunc(funcName, arguments)
                    //  .OnSuccess(delegate(JToken res) { result = res.ToObject(retType); })
                    //  .Wait();
                    //return result;
                }
            });

            return Dynamic.CoerceToDelegate(genericWrapper, delegateType);
        }

        private ClientFunction CreateFuncWrapperDelegate(string remoteCallbackUUID)
        {
            return (ClientFunction)delegate(object[] arguments)
            {
                return CallClientFunction(remoteCallbackUUID, arguments);
            };
        }

        private void SendCallReply(int callID, Delegate nativeMethod, bool success, object retValue, object exception)
        {
            List<object> callReplyMessage = new List<object>();
            callReplyMessage.Add("call-reply");
            callReplyMessage.Add(callID);
            callReplyMessage.Add(success);
            if (!success)
                callReplyMessage.Add(exception);
            else if (nativeMethod.Method.ReturnType != typeof(void))
                callReplyMessage.Add(retValue);
            SendMessage(callReplyMessage);
        }

        private void SendCallError(int callID, string reason)
        {
            IMessage messageObject = new MessageBase();
            List<object> errorReplyMessage = new List<object>();
            errorReplyMessage.Add("call-error");
            errorReplyMessage.Add(callID);
            errorReplyMessage.Add(reason);
            SendMessage(errorReplyMessage);
        }

        private void HandleCallError(IMessage errorMessage)
        {
            int callID = errorMessage.ID;
            string reason = (string)errorMessage.Result;

            // Call error with callID = -1 means we've sent something that was not understood by other side or was
            // malformed. This probably means that protocols aren't incompatible or incorrectly implemented on either
            // side.
            if (callID == -1)
                throw new Exception(reason);

            FuncCallBase failedCall = null;
            lock (activeCalls)
            {
                if (activeCalls.ContainsKey(callID))
                {
                    failedCall = activeCalls[callID];
                    activeCalls.Remove(callID);
                }
            }

            if (failedCall != null)
                failedCall.HandleError(reason);
            else
                SendCallError(-1, "Invalid callID: " + callID);
        }

        private void HandleCallResponse(IMessage responseMessage)
        {
            int callID = responseMessage.ID;

            FuncCallBase completedCall = null;
            lock (activeCalls)
            {
                if (activeCalls.ContainsKey(callID))
                {
                    completedCall = activeCalls[callID];
                    activeCalls.Remove(callID);
                }
            }

            if (completedCall != null)
            {
                bool success = !responseMessage.IsException;
                object result = responseMessage.Result;
                if (success)
                    completedCall.HandleSuccess(result);
                else
                    completedCall.HandleException(new Exception(result as string));
            }
            else
            {
                SendCallError(-1, "Invalid callID: " + callID);
            }
        }

        private int getValidCallID()
        {
            lock (nextCallIDLock)
            {
                return nextCallID++;
            }
        }

        private bool IsOneWay(string qualifiedMethodName)
        {
            List<string> onewayMethods = new List<string>
            {
                // Add new one-way calls here
            };

            return onewayMethods.Contains(qualifiedMethodName);
        }

        private List<object> createCallMessage(int callID, string name, List<int> callbacks, List<object> convertedArgs)
        {
            List<object> callMessage = new List<object>();
            callMessage.Add("call");
            callMessage.Add(callID);
            callMessage.Add(name);
            // Add a list of callback indicies.
            callMessage.Add(callbacks);
            // Add converted arguments.
            callMessage.AddRange(convertedArgs);

            return callMessage;
        }

        private List<object> convertCallbackArguments(object[] args, out List<int> callbacks)
        {
            callbacks = createCallbacksFromArguments(args);

            List<object> convertedArgs = new List<object>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is Delegate)
                {
                    var arg = args[i] as Delegate;
                    string callbackGuid = null;
                    lock (registeredCallbacks)
                        callbackGuid = registeredCallbacks[arg];
                    convertedArgs.Add(callbackGuid);
                }
                else
                {
                    convertedArgs.Add(args[i]);
                }
            }
            return convertedArgs;
        }

        private List<int> createCallbacksFromArguments(object[] args)
        {
            List<int> callbacks = new List<int>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is Delegate)
                {
                    var arg = args[i] as Delegate;

                    string callbackGuid = null;
                    lock (registeredCallbacks)
                    {
                        if (!registeredCallbacks.ContainsKey(arg))
                        {
                            callbackGuid = Guid.NewGuid().ToString();
                            registeredCallbacks[arg] = callbackGuid;
                        }
                        else
                        {
                            callbackGuid = registeredCallbacks[arg];
                        }
                    }

                    lock (registeredFunctions)
                        registeredFunctions[callbackGuid] = arg;

                    callbacks.Add(i);
                }
            }
            return callbacks;
        }

        private object nextCallIDLock = new object();
        private int nextCallID = 0;

        private Dictionary<int, FuncCallBase> activeCalls = new Dictionary<int, FuncCallBase>();
        private Dictionary<string, Delegate> registeredFunctions = new Dictionary<string, Delegate>();
        private Dictionary<Delegate, string> registeredCallbacks = new Dictionary<Delegate, string>();
        private JsonSerializerSettings settings = new JsonSerializerSettings();
        private JsonSerializer serializer;

        private ISocket socket;

        private static Logger logger = LogManager.GetCurrentClassLogger();

        internal IWSJFuncCallFactory wsjFuncCallFactory = new WSJFuncCallFactory();
    }
}
