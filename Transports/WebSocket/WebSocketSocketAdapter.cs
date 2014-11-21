using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocket4Net;
using SuperSocket.ClientEngine;
using KIARA;

namespace KIARA.Transport.WebSocketTransport
{
    public class WebSocketSocketAdapter :  ITransportConnection
    {
        public WebSocketSocketAdapter(string uri)
        {
            this.WebSocket = new WebSocket(uri);
            WebSocket.MessageReceived += HandleMessageReceived;
            WebSocket.Error += HandleError;
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
        public new event EventHandler<TransportErrorEventArgs> Error;

        public bool IsConnected
        {
            get { return WebSocket.State == WebSocketState.Open; }
        }

        private WebSocket WebSocket;

        public event EventHandler Opened;

        public event EventHandler Closed;

        event EventHandler<TransportErrorEventArgs> ITransportConnection.Error
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        event EventHandler<TransportMessageEventArgs> ITransportConnection.Message
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Send(object message)
        {
            throw new NotImplementedException();
        }
    }
}
