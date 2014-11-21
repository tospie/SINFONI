using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class TransportErrorEventArgs : EventArgs
    {
        public TransportErrorEventArgs() { }

        public TransportErrorEventArgs(Exception e)
        {
            Exception = e;
        }

        public Exception Exception { get; private set; }
    }
}
