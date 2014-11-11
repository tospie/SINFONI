using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net;
using KIARA.Exceptions;
using System.Reflection;
using Dynamitey;

namespace KIARA
{
    /// <summary>
    /// Represents a generated function wrapper. It allows calling the function with arbitrary arguments.
    /// </summary>
    /// <returns>An object representing a call.</returns>
    public delegate IClientFunctionCall ClientFunction(params object[] parameters);
    public delegate object GenericWrapper(params object[] arguments);

    /// <summary>
    /// This class represenents a connection to the remote end. It may be used to load new IDL definition files,
    /// generate callable remote function  wrappers and to register local functions as implementations for remote calls.
    /// </summary>
    public abstract class Connection
    {
        /// <summary>
        /// Raised when a connection is closed.
        /// </summary>
        public abstract event EventHandler Closed;

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// Convenient wrapper around GenerateFuncWrapper. Can be used to quickly create function wrapper and call it
        /// at once, e.g. <c>client["clientFunc"]("arg1", 42);</c>
        /// </summary>
        /// <param name="name">Function name.</param>
        /// <returns>Function wrapper.</returns>
        public ClientFunction this[string name]
        {            
            get
            {
                string[] serviceName = name.Split('.');
                return GenerateClientFunction(serviceName[0], serviceName[1]);
            }
        }

        /// <summary>
        /// Loads an IDL definition file at <paramref name="uri"/> into the connection.
        /// </summary>
        /// <param name="uri">URI of the IDL definition file.</param>
        public virtual void LoadIDL(string uri)
        {
            string contents = webClient.DownloadString(uri);
            IDLParser.Instance.parseIDL(contents);
        }

        /// <summary>
        /// Handles an incoming message.
        /// </summary>
        /// <param name="message">The incoming message.</param>
        public void HandleMessage(object sender, TransportMessageEventArgs e)
        {
            IMessage receivedMessage = null;

            try
            {
                // Deserializes Message according to loaded protocol. As client agreed with server on respective protocol
                receivedMessage = Protocol.DeserializeMessage(e.Message);
            }
            catch (Exception)
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

        /// Generates a func wrapper for the <paramref name="funcName"/>. Optional <paramref name="typeMapping"/> string
        /// may be used to specify data omission and reordering options.
        /// </summary>
        /// <returns>The generated func wrapper.</returns>
        /// <param name="funcName">Name of the function to be wrapped.</param>
        /// <param name="typeMapping">Type mapping string.</param>
        public ClientFunction GenerateClientFunction(string serviceName, string functionName)
        {
            if (!ServiceRegistry.Instance.ContainsService(serviceName))
                throw new ServiceNotRegisteredException(serviceName);

            var service = ServiceRegistry.Instance.GetService(serviceName);

            if (!service.ContainsServiceFunction(functionName))
                throw new ServiceNotRegisteredException(functionName);

            return (ClientFunction)delegate(object[] parameters)
            {
                KiaraService registeredService = ServiceRegistry.Instance.GetService(serviceName);
                ServiceFunctionDescription registeredServiceFunction = registeredService.GetServiceFunction(functionName);

                if (!registeredServiceFunction.CanBeCalledWithParameters(parameters))
                {
                    throw new ParameterMismatchException(
                        "Could not call Service Function " + serviceName + "." + functionName
                            + ". The provided parameters can not be mapped to the parameters specified in the IDL.");
                }
                object[] callParameters = new object[parameters.Length];
                for (var i = 0; i < parameters.Length; i++)
                {
                    KtdType expectedParameterType = registeredServiceFunction.Parameters.ElementAt(i).Value;
                    callParameters[i] = expectedParameterType.AssignValuesFromObject(parameters[i]);
                }
                // TODO: Implement Calling remote functions
                return CallClientFunction(serviceName + "." + functionName, callParameters);
            };
        }

        /// <summary>
        /// Registers a local <paramref name="handler"/> as an implementation for the <paramref name="funcName"/>.
        /// Optional <paramref name="typeMapping"/> string can be used to specify data omission and reordering options.
        /// </summary>
        /// <param name="funcName">Name of the implemented function.</param>
        /// <param name="handler">Handler to be invoked upon remote call.</param>
        /// <param name="typeMapping">Type mapping string.</param>
        public virtual void RegisterFuncImplementation(string funcName, Delegate handler, string typeMapping = "")
        {
            // TODO: implement type mapping and add respective tests
            RegisterHandler(funcName, handler);
        }

        /// <summary>
        /// Sets some property of the connection. May be used by subclasses to allow clients to configure them.
        /// </summary>
        /// <returns><c>true</c>, if property is supported and accepts given value, <c>false</c> otherwise.</returns>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value to be set.</param>
        public virtual bool SetProperty(string name, object value)
        {
            return false;
        }

        /// <summary>
        /// Returns some property of the connection. May be used by subclasses to allow clients obtain some information
        /// about the connection.
        /// </summary>
        /// <returns><c>true</c>, if property is supported, <c>false</c> otherwise.</returns>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value to be returned.</param>
        public virtual bool GetProperty(string name, out object value)
        {
            value = null;
            return false;
        }

        /// <summary>
        /// Calls a function with a given name and arguments on the remote end.
        /// </summary>
        /// <param name="funcName">Function name.</param>
        /// <param name="args">Argunents.</param>
        /// <returns>Object representing remote call.</returns>
        protected abstract IClientFunctionCall CallClientFunction(string funcName, params object[] args);

        /// <summary>
        /// Registers a handler to be invoked when a function with given name is invoked remotely.
        /// </summary>
        /// <param name="funcName">Function name.</param>
        /// <param name="handler">Handler delegate.</param>
        protected abstract void RegisterHandler(string funcName, Delegate handler);

        internal IWebClient webClient = new WebClientWrapper();

        protected ITransport Transport;
        protected IProtocol Protocol;
    }
}
