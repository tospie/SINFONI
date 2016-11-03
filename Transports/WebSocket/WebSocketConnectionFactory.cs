// This file is part of SINFONI.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Newtonsoft.Json.Linq;
using WebSocket4Net;
using System.Net;
using NLog;
using SuperSocket.SocketBase.Config;
using SINFONI;
using System.Net.Sockets;

namespace SINFONI.Transport.WebSocketTransport
{
    /// <summary>
    /// WebSocketJSON connection factory implementation.
    /// </summary>
    public class WebSocketConnectionFactory : ITransportConnectionFactory
    {
        #region IConnectionFactory implementation

        public ITransportConnection OpenConnection(string host, int port, Context context, Action<Connection> onConnected)
        {
            if (port == -1 || host == null)
                throw new Error(ErrorCode.CONNECTION_ERROR, "No port and/or IP address is present in configuration.");

            ITransportConnection transportConnection = webSocketFactory.Construct("ws://" + host + ":" + port + "/");
            transportConnection.Error += (sender, e) =>
            {
                Console.WriteLine("Error in connection to " + host + ":" + port + ":   " + e.Exception.Message);
            };
            try
            {
                transportConnection.Open();
            }
            catch(Exception e)
            {
                Console.WriteLine("Failed to open Socket connection to ws://{0}:{1}, Reason: {2} ", host, port, e.Message);
            }
            return transportConnection;
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

        #endregion

        internal IWSJServerFactory wsjServerFactory = new WSJServerFactory();
        internal IWebSocketFactory webSocketFactory = new WebSocketFactory();

        private static Logger logger = LogManager.GetCurrentClassLogger();
    }
}

