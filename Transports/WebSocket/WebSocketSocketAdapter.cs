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
using System.Linq;
using System.Text;
using WebSocket4Net;
using SuperSocket.ClientEngine;
using SINFONI;

namespace SINFONI.Transport.WebSocketTransport
{
    public class WebSocketSocketAdapter : ITransportConnection
    {
        public WebSocketSocketAdapter(string uri)
        {
            this.WebSocket = new WebSocket(uri);
            WebSocket.MessageReceived += HandleMessageReceived;
            WebSocket.Error += HandleError;
            this.WebSocket.Opened += (o, e) =>
            {
                if (this.Opened != null)
                    this.Opened(this, e);
            };
            WebSocket.Closed += (o, e) =>
            {
                if (this.Closed != null)
                    this.Closed(this, e);
            };
        }

        private void HandleError(object sender, ErrorEventArgs e)
        {
            if (Error != null)
                Error(sender, new TransportErrorEventArgs(e.Exception));
        }

        void HandleMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (Message != null)
                Message(sender, new TransportMessageEventArgs(e.Message));
        }

        public event EventHandler<TransportMessageEventArgs> Message;
        public event EventHandler<TransportErrorEventArgs> Error;

        public bool IsConnected
        {
            get { return WebSocket.State == WebSocketState.Open; }
        }

        private WebSocket WebSocket;

        public event EventHandler Opened;

        public event EventHandler Closed;

        public void Send(object message)
        {
            if (message.GetType() == typeof(string))
                this.WebSocket.Send((string)message);
            else
            {
                byte[] byteMessage = (byte[])message;
                this.WebSocket.Send(byteMessage, 0, byteMessage.Length);
            }
        }

        public void Open()
        {
            this.WebSocket.Open();
        }

        public void Close()
        {
            this.WebSocket.Close();
        }
    }
}
