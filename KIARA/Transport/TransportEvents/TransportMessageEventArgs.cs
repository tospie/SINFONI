using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class TransportMessageEventArgs : EventArgs
    {
        public IMessage Message { get;  }
    }
}
