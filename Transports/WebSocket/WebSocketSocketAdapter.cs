using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocket4Net;
using SuperSocket.ClientEngine;
using KIARA;

namespace KIARA.Transport.WebSocketTransport
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
            this.WebSocket.Send((string)message);
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
