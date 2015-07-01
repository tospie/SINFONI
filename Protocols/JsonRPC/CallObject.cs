using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI.Protocols.JsonRPC
{
    public class CallObject
    {
        public string method;
        public string jsonrpc = "2.0";
        public object[] parameters;
        public int id;
        public JsonRpcError error;
        public object result;
    }
}
