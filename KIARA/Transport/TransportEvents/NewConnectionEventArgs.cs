using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class NewConnectionEventArgs : EventArgs
    {
        public NewConnectionEventArgs(Connection connection)
        {
            Connection = connection;
        }

        public Connection Connection { get; private set; }
    }
}
