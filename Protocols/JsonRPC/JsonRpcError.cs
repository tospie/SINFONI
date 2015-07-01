using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI.Protocols.JsonRPC
{
    public class JsonRpcError
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
