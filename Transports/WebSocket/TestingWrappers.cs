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
using SINFONI;

namespace SINFONI.Transport.WebSocketTransport
{
    public interface IWSJServerFactory
    {
        IWSServer Construct(Action<Connection> onNewClient);
    }

    public class WSJServerFactory : IWSJServerFactory
    {
        public IWSServer Construct(Action<Connection> onNewClient)
        {
            return new WSServer();
        }
    }

    public interface IWSJFuncCallFactory
    {
        FuncCallBase Construct(string serviceName, string methodName, Connection connection);
    }

    public class WSJFuncCallFactory : IWSJFuncCallFactory
    {
        public FuncCallBase Construct(string serviceName, string methodName, Connection connection)
        {
            return new FuncCallBase(serviceName, methodName, connection);
        }
    }

    public interface IWebSocketFactory
    {
        ITransportConnection Construct(string uri);
    }

    public class WebSocketFactory : IWebSocketFactory
    {
        public ITransportConnection Construct(string uri)
        {
            return new WebSocketSocketAdapter(uri);
        }
    }
}

