using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class MessageBase : IMessage
    {
        public MessageType Type { get; private set; }

        public int ID { get; private set; }

        public string MethodName { get; private set; }

        public List<object> Parameters { get; private set; }

        public List<int> Callbacks { get; private set; }

        public object Result { get; private set; }

        public bool IsException { get; private set;}
    }
}
