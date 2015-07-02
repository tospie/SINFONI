﻿// This file is part of SINFONI.
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

namespace SINFONI
{
    /// <summary>
    /// The interface that should be implemented by the connection factories in protocols. Used to initiate connections 
    /// and construct <see cref="Connection"/> objects of the respective protocol.
    /// </summary>
    public interface ITransportConnectionFactory
    {
        /// <summary>
        /// Opens a connection to the remote server specified by the <paramref name="serverConfig"/> and executes
        /// <paramref name="onConnected"/> when the connection is established.
        /// </summary>
        /// <param name="serverConfig">Server config.</param>
        /// <param name="onConnected">Callback to be called when the connection is established.</param>
        ITransportConnection OpenConnection(string host, int port, Context context, Action<Connection> onConnected);

        /// <summary>
        /// Starts the server listening for new clients according to the configuration in the
        /// <paramref name="serverConfig"/>. For each new client <paramref name="onNewClient"/> is called.
        /// </summary>
        /// <param name="serverConfig">Server config.</param>
        /// <param name="onNewClient">Callback to be called for each new client.</param>
        ITransportListener StartConnectionListener(string uri, int port);
    }
}

