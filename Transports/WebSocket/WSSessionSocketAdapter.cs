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
using SINFONI;

namespace SINFONI.Transport.WebSocketTransport
{
    class WSSessionSocketAdapter : ITransportConnection
    {
        public WSSessionSocketAdapter(WSSession aSession)
        {
            session = aSession;
            session.Closed += HandleClosed;
            session.MessageReceived += HandleMessageReceived;
            session.DataReceived += HandleDataReceivedReceived;
        }

        // This is actually another layer of message here, consider !! The message we receive here is some
        // result of the serialized message object that we use internally. The "Message" event should thus
        // redirect the received message to the de-serializer which then de-serializes the received string
        // to the SINFONI.Message - object
        void HandleMessageReceived(object sender, WebSocket4Net.MessageReceivedEventArgs e)
        {
            if (Message != null)
                Message(sender, new TransportMessageEventArgs(e.Message));
        }

        void HandleDataReceivedReceived(object sender, WebSocket4Net.DataReceivedEventArgs e)
        {
            if (Message != null)
                Message(sender, new TransportMessageEventArgs(e.Data));
        }

        void HandleClosed(object sender, EventArgs e)
        {
            if (Closed != null)
                Closed(sender, e);
        }

        public event EventHandler Closed;

        public event EventHandler<TransportMessageEventArgs> Message;

        public bool IsConnected
        {
            get { return session.Connected; }
        }

        public void Close()
        {
            session.Close();
        }

        public void Send(object message)
        {
            if (message.GetType() == typeof(string))
                session.Send((string)message);
            else
            {
                byte[] byteMessage = (byte[])message;
                session.Send(byteMessage, 0, byteMessage.Length);
            }
        }

        WSSession session;

        #region This part of the interface is only used for client sockets
        public event EventHandler Opened
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public event EventHandler<TransportErrorEventArgs> Error
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public void Open()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
