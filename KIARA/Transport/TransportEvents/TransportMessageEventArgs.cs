using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class TransportMessageEventArgs : EventArgs
    {
        public TransportMessageEventArgs(IMessage message)
        {
            this.message = message;
        }

        public IMessage Message {
            get { return message; }
        }

        private IMessage message;
    }
}
