using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using KIARA;

namespace HttpJSONProtocol
{
    public class HttpJSONConnectionFactory : IConnectionFactory
    {
        #region IConnectionFactoryImplementation
        public void OpenConnection(Server serverConfig, Context context, Action<Connection> onConnected)
        {
            int port = ProtocolUtils.retrieveProtocolSetting(serverConfig, "port", -1);
            string host = ProtocolUtils.retrieveProtocolSetting(serverConfig, "host", (string)null);

            if (port == -1 || host == null)
                throw new Error(ErrorCode.CONNECTION_ERROR, "No port and/or IP address is present in configuration.");

            HttpJSONConnection connection = new HttpJSONConnection();
            connection.RemoteHost = host;
            connection.RemotePort = port;

            onConnected(connection);
        }

        public void StartServer(Server serverConfig, Context context, Action<Connection> onNewClient)
        {
            int port = ProtocolUtils.retrieveProtocolSetting(serverConfig, "port", 34580);
            string host = ProtocolUtils.retrieveProtocolSetting(serverConfig, "host", "Any");

            if (host != "Any")
            {
                IPAddress[] ipAddresses = Dns.GetHostAddresses(host);
                if (ipAddresses.Length == 0)
                    throw new Error(ErrorCode.CONNECTION_ERROR, "Cannot identify IP address by hostname.");

                string serverIP = ipAddresses[0].ToString();
                HttpJSONServer server = new HttpJSONServer(onNewClient);
                server.StartServer(serverIP, port);
            }
            else
            {
                HttpJSONServer server = new HttpJSONServer(onNewClient);
                server.StartServer("localhost", port);
            }
        }

        public string Name
        {
            get { return "http-json"; }
        }
        #endregion
    }
}
