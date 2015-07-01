using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA;

namespace KIARA.Transport.WebSocketTransport
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
        // to the KIARA.Message - object
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
