using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocket4Net;

namespace KIARA.Transport.WebSocketTransport
{
    public class WebSocketTransportConnection : ITransportConnectionFactory
    {

        public void OpenConnection(string uri, Context context, Action<Connection> onConnected)
        {                     
            if (uri == null)
                throw new Error(ErrorCode.CONNECTION_ERROR, "No port and/or IP address is present in configuration.");

            WebSocket connectionSocket = new WebSocket(uri);
            
            //ITransportConnection transportConnection = webSocketFactory.Construct("ws://" + host + ":" + port + "/");
            connectionSocket.Opened += (sender, e) => onConnected(new Connection());
            connectionSocket.Error += (sender, e) =>
            {
                logger.WarnException("Error in connection to " + uri, e.Exception);
            };
            connectionSocket.Open();
        }

        public void StartServer(string uri, Context context, Action<Connection> onNewClient)
        {
            WebSocket serverSocket = new WebSocket(uri);
            serverSocket.MessageReceived
            ServerConfig config = new ServerConfig();
            config.Port = ProtocolUtils.retrieveProtocolSetting(serverConfig, "port", 34837);
            config.MaxRequestLength = 100000;

            string host = ProtocolUtils.retrieveProtocolSetting(serverConfig, "host", "Any");
            if (host != "Any")
            {
                IPAddress[] ipAddresses = Dns.GetHostAddresses(host);
                if (ipAddresses.Length == 0)
                    throw new Error(ErrorCode.CONNECTION_ERROR, "Cannot identify IP address by hostname.");
                config.Ip = ipAddresses[0].ToString();  // we take first entry as it does not matter which one is used
            }
            else
            {
                config.Ip = "Any";
            }

            IWSJServer server = wsjServerFactory.Construct(onNewClient);
            server.Setup(config);
            server.Start();
        }

        public string Name
        {
            get
            {
                return "websocket";
            }
        }

        private static Logger logger = LogManager.GetCurrentClassLogger();
    }
}
