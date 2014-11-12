using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class MessageBase : IMessage
    {
        public MessageType Type { get; internal set; }

        public int ID { get; internal set; }

        public string MethodName { get; internal set; }

        public List<object> Parameters { get; internal set; }

        public List<int> Callbacks { get; internal set; }

        public object Result { get; internal set; }

        public bool IsException { get; internal set; }
    }
}
