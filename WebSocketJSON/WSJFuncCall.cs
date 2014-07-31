using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Diagnostics;
using KIARA;
using Newtonsoft.Json;

namespace WebSocketJSON
{
    #region Testing

    public interface IWSJFuncCall : IClientFunctionCall
    {
        void HandleSuccess(JToken retValue);
        void HandleException(JToken exception);
        void HandleError(string error);
    }

    #endregion

    /// <summary>
    /// Call object implementation for WebSocketJSON protocol.
    /// </summary>
    public class WSJFuncCall : FuncCallBase, IWSJFuncCall
    {
        public WSJFuncCall(string serviceName, string methodName)
            : base(serviceName, methodName)
        {
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Converters.Add(new StandardFloatConverter());
            settings.ContractResolver = new PrivateSetterResolver();
            serializer = JsonSerializer.Create(settings);
        }

        protected override object ConvertResult(object result, Type type)
        {
            KtdType idlReturnType = ServiceRegistry.Instance
                        .GetService(ServiceName)
                        .GetServiceFunction(FunctionName)
                        .ReturnType;

            var convertedResult = idlReturnType.AssignValuesToNativeType(result, type);
            return convertedResult;
        }

        /// <summary>
        /// Handles the successful completion of the call.
        /// </summary>
        /// <param name="retValue">Ret value returned from the call.</param>
        public void HandleSuccess(JToken retValue)
        {
            try
            {
                base.HandleSuccess(retValue.ToObject<Dictionary<string, object>>(serializer));
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Handles the exception thrown from the call.
        /// </summary>
        /// <param name="exception">Exception that was thrown.</param>
        public void HandleException(JToken exception)
        {
            base.HandleException(new Exception(exception.ToString()));
        }

        private JsonSerializerSettings settings = new JsonSerializerSettings();
        private JsonSerializer serializer;
    }
    
}
