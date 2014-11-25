using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA.Protocols.JsonRPC
{
    public class JsonRpcError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
