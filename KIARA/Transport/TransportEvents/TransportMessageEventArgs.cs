using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class TransportMessageEventArgs : EventArgs
    {
        public TransportMessageEventArgs(object message)
        {
            this.message = message;
        }

        public object Message {
            get { return message; }
        }

        private object message;
    }
}
