using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA;

namespace WebSocketJSON
{
    public class WSMessage : MessageBase
    {
        public object Payload {
            get { return payload;}
        }

        public WSMessage(string message)
        {
            payload = message;
        }

        object payload;
    }
}
