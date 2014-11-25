using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class NewConnectionEventArgs : EventArgs
    {
        public NewConnectionEventArgs(ITransportConnection connection)
        {
            Connection = connection;
        }

        public ITransportConnection Connection { get; private set; }
    }
}
