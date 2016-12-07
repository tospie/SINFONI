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

namespace SINFONI
{
    /// <summary>
    /// The interface that should be implemented by the connection factories in protocols. Used to initiate connections 
    /// and construct <see cref="Connection"/> objects of the respective protocol.
    /// </summary>
    public interface ITransportConnectionFactory
    {
        /// <summary>
        /// Creates a TransportConnection object which event handlers can be assigned to. That object can then be used
        /// to open the connection to the remote server.
        /// </summary>
        /// <param name="host">IP Address or hostname of the server</param>
        /// <param name="port">Port under which remote server is listening</param>
        /// <param name="context">Context under which the connection is openend</param>
        ITransportConnection CreateTransportConnection(string host, int port, Context context);

        /// <summary>
        /// Starts the server listening for new clients
        /// </summary>
        /// <param name="uri">URI (hostname or IP address) of the new server.</param>
        /// <param name="port">Port of the new server</param>
        ITransportListener StartConnectionListener(string uri, int port);
    }
}

