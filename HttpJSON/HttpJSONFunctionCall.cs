using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            throw new NotImplementedException();
        }
    }
}
