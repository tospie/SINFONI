using System;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA;
using System.Net;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;

namespace HttpJSONProtocol
{
    class HttpJSONConnection : Connection
    {
        public override event EventHandler Closed;

        public override void Disconnect()
        {
            // HTTP JSON is Stateless and does therefore not implement any connection- or disconnection behaviour
        }

        protected override void ProcessIDL(string parsedIDL)
        {
            // Implemented in Baseclass for Connection
        }

        protected override IClientFunctionCall CallClientFunction(string funcName, params object[] args)
        {
            int callId = getValidCallID();
            using(var postClient = new WebClient())
            {
                CallObject functionCall = new CallObject();
                functionCall.method = funcName;
                functionCall.id = callId;
                functionCall.parameters = args;
                string serializedCall = JsonSerializer.Serialize(functionCall);
                response = postClient.UploadString("http://" + RemoteHost + ":" + RemotePort + "/", "POST", serializedCall);
            }

            string[] serviceNames = funcName.Split('.');
            lastFuncCall = new HttpJSONFunctionCall(serviceNames[0], serviceNames[1]);
            Thread asynchCall = new Thread(fakeAsynch);
            asynchCall.Start();
            return lastFuncCall;
        }

        private void fakeAsynch()
        {
            Thread.Sleep(1000);
            lastFuncCall.HandleSuccess(response);
        }

        protected override void RegisterHandler(string funcName, Delegate handler)
        {
            lock (registeredFunctions)
                registeredFunctions[funcName] = handler;
        }

        internal object HandleRequest(CallObject data)
        {
            string method = data.method;
            Delegate nativeMethod = registeredFunctions[method];
            ParameterInfo[] nativeParameters = nativeMethod.Method.GetParameters();
            object[] nativeArgs = new object[nativeParameters.Length];

            object[] parameters = data.parameters;

            ServiceFunctionDescription calledFunction = ServiceRegistry.Instance
                .GetService(method.Split('.')[0])
                .GetServiceFunction(method.Split('.')[1]);

            KtdType[] idlTypes = calledFunction.Parameters.Values.ToArray();

            for (var i = 0; i < parameters.Length; i++)
            {
                KtdType idlType = idlTypes[i];
                nativeArgs[i] = idlType.AssignValuesToNativeType(parameters[i], nativeParameters[i].ParameterType);
            }
            return nativeMethod.DynamicInvoke(nativeArgs);
        }

        private int getValidCallID()
        {
            lock (nextCallIDLock)
            {
                return nextCallID++;
            }
        }

        private object nextCallIDLock = new object();
        private int nextCallID = 0;

        internal string RemoteHost;
        internal int RemotePort;
        JavaScriptSerializer JsonSerializer = new JavaScriptSerializer();

        HttpJSONFunctionCall lastFuncCall;
        object response;

        private Dictionary<string, Delegate> registeredFunctions = new Dictionary<string, Delegate>();
    }
}
