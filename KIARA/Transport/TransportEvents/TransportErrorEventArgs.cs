using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class TransportErrorEventArgs : EventArgs
    {
        public Exception Exception { get; private set; }
    }
}
