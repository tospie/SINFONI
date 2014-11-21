using System;
using Newtonsoft.Json.Linq;
using WebSocket4Net;
using System.Net;
using NLog;
using SuperSocket.SocketBase.Config;
using KIARA;

namespace KIARA.Transport.WebSocketTransport
{
    /// <summary>
    /// WebSocketJSON connection factory implementation.
    /// </summary>
    public class WebSocketConnectionFactory : ITransportConnectionFactory
    {
        #region IConnectionFactory implementation

        public void OpenConnection(string host, int port, Context context, Action<Connection> onConnected)
        {
            if (port == -1 || host == null)
                throw new Error(ErrorCode.CONNECTION_ERROR, "No port and/or IP address is present in configuration.");

            ITransportConnection transportConnection = webSocketFactory.Construct("ws://" + host + ":" + port + "/");
            transportConnection.Opened += (sender, e) => onConnected(new Connection(transportConnection));
            transportConnection.Error += (sender, e) => {
                logger.WarnException("Error in connection to " + host + ":" + port, e.Exception);
            };
            transportConnection.Open();
        }

        public ITransportListener StartConnectionListener(string uri, int port)
        {
            ServerConfig config = new ServerConfig();
            config.Port = port;
            config.MaxRequestLength = 100000;

            string host = uri;
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
            var server = new WSServer();
            server.Setup(config);
            server.Start();
            return server as ITransportListener;
        }

        public string Name
        {
            get
            {
                return "websocket-json";
            }
        }

        #endregion

        internal IWSJServerFactory wsjServerFactory = new WSJServerFactory();
        internal IWebSocketFactory webSocketFactory = new WebSocketFactory();

        private static Logger logger = LogManager.GetCurrentClassLogger();
    }
}

