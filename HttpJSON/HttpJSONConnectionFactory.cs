using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA;

namespace HttpJSONProtocol
{
    class HttpJSONConnectionFactory : IConnectionFactory
    {
        public void OpenConnection(Server serverConfig, Context context, Action<Connection> onConnected)
        {
            throw new NotImplementedException();
        }

        public void StartServer(Server serverConfig, Context context, Action<Connection> onNewClient)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }
    }
}
