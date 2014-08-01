using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpJSONProtocol
{
    public class CallObject
    {
        public string method;
        public string jsonrpc = "2.0";
        public object[] parameters;
        public int id;
    }
}
