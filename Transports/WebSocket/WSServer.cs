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
using SuperWebSocket;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Logging;
using SuperWebSocket.Protocol;
using SuperSocket.SocketBase.Config;
using SINFONI;

namespace SINFONI.Transport.WebSocketTransport
{
    #region Testing
    public interface IWSServer
    {
        bool Setup(IServerConfig config, ISocketServerFactory socketServerFactory = null,
            IReceiveFilterFactory<IWebSocketFragment> receiveFilterFactory = null, ILogFactory logFactory = null,
            IEnumerable<IConnectionFilter> connectionFilters = null,
            IEnumerable<SuperSocket.SocketBase.Command.ICommandLoader> commandLoaders = null);
        bool Start();
    }
    #endregion

    /// <summary>
    /// A simple WebSocket server based on SuperWebSocket library.
    /// </summary>
    public class WSServer : WebSocketServer<WSSession>, IWSServer, ITransportListener
    {
        public event EventHandler<NewConnectionEventArgs> NewClientConnected;
        /// <summary>
        /// Initializes a new instance of the <see cref="WebSocketJSON.WSJServer"/> class.
        /// </summary>
        /// <param name="onNewClient">The handler to be called for each new client.</param>
        public WSServer()
        {

            NewSessionConnected += (session) =>
            {
                session.SocketSession.Closed += (genericSession, reason) => session.HandleClosed(reason.ToString());
                var socketAdapter = new WSSessionSocketAdapter(session);

                if (NewClientConnected != null)
                {
                    NewClientConnected(this, new NewConnectionEventArgs(socketAdapter));
                }
            };
            NewMessageReceived += (session, value) => session.HandleMessageReceived(value);
            NewDataReceived += (session, data) => session.HandleDataReceived(data);
        }

    }
}

