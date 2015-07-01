using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI
{
    public interface ITransportConnection
    {
        /// <summary>
        /// Raised when the connection is opened.
        /// </summary>
        event EventHandler Opened;

        /// <summary>
        /// Raised when the connection is closed.
        /// </summary>
        event EventHandler Closed;

        /// <summary>
        /// Raised when an error is occured in the connection.
        /// </summary>
        event EventHandler<TransportErrorEventArgs> Error;

        /// <summary>
        /// Raised when a message is received by the connection.
        /// </summary>
        event EventHandler<TransportMessageEventArgs> Message;

        /// <summary>
        /// Returns true if the socket is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Opens the connection.
        /// </summary>
        void Open();

        /// <summary>
        /// Closes the connection.
        /// </summary>
        void Close();

        /// <summary>
        /// Sends a message over this socket.
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        void Send(object message);
    }
}
