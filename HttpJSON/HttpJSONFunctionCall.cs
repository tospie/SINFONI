using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using KIARA;

namespace HttpJSONProtocol
{
    public class HttpJSONFunctionCall : FuncCallBase, IClientFunctionCall
    {
        public HttpJSONFunctionCall(string serviceName, string methodName)
            : base(serviceName, methodName)
        { }

        protected override object ConvertResult(object result, Type type)
        {
            var service = ServiceRegistry.Instance.GetService(this.ServiceName);
            var function = service.GetServiceFunction(this.FunctionName);
            Dictionary<string, object> resultAsDictionary =
                new JavaScriptSerializer().Deserialize<Dictionary<string, object>>(result as string);

            KtdType idlReturnType = function.ReturnType;
            return idlReturnType.AssignValuesToNativeType(resultAsDictionary, type);
        }
    }
}
