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
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using SuperWebSocket;
using Newtonsoft.Json;
using System.Reflection;
using Dynamitey;
using System.Runtime.InteropServices;
using WebSocket4Net;
using NLog;
using SINFONI;
using System.Text;

namespace SINFONI.Transport.WebSocketTransport
{
    /// <summary>
    /// WebSocketJSON session implementation. Contains Connection adapter for SINFONI.
    /// </summary>
    public class WSSession : WebSocketSession<WSSession>
    {
        public event EventHandler<ClosedEventArgs> Closed;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler<DataReceivedEventArgs> DataReceived;

        protected override void HandleException(Exception e)
        {
            this.Send("[WSSession] Unhandled exception in Websocket Session: " + e);
        }

        public void HandleClosed(string reason)
        {
            if (Closed != null)
                Closed(this, new ClosedEventArgs(reason));

            if (!reason.Equals("ClientClosing"))
                logger.Warn("Connection closed: " + reason);
        }

        public void HandleMessageReceived(string message)
        {
            if (MessageReceived != null)
                MessageReceived(this, new MessageReceivedEventArgs(message));
        }

        public void HandleDataReceived(byte[] data)
        {
            if (DataReceived != null)
                DataReceived(this, new DataReceivedEventArgs(data));
        }

        private static Logger logger = LogManager.GetCurrentClassLogger();
    }
}

