using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA;

namespace WebSocketJSON
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(IMessage message)
        {
            Message = message;
        }

        public IMessage Message { get; private set; }
    }
}
