using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class MessageBase : IMessage
    {
        public MessageType Type { get; set; }

        public int ID { get; set; }

        public string MethodName { get; set; }

        public List<object> Parameters { get; set; }

        public List<int> Callbacks { get; set; }

        public object Result { get; set; }

        public bool IsException { get; set; }
    }
}
