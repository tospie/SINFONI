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
using System.IO;
using System.Linq;
using System.Text;
using SINFONI;
using SINFONI.Transport.WebSocketTransport;
using SINFONI.Protocols.JsonRPC;
using FiVESJson;


namespace SimpleServer
{
    public class SimpleServer
    {
        public struct Vector
        {
            public float x;
            public float y;
            public float z;
        };

        public SimpleServer()
        {
            ITransport websocketTransport = new WebSocketTransport();
            TransportRegistry.Instance.RegisterTransport(websocketTransport);

            IProtocol jsonRpc = new JsonRpcProtocol();
            IProtocol fivesJson = new FiVESJsonProtocol();
            ProtocolRegistry.Instance.RegisterProtocol(jsonRpc);
            ProtocolRegistry.Instance.RegisterProtocol(fivesJson);

            SINFONIServer newServer = new SINFONIServer("localhost", 8080, "/service/", "server.sinfoni");
            var service = newServer.StartService("127.0.0.1", 34568, "/service", "ws", "jsonrpc");

            service.OnNewClient += new NewClient(HandleNewClient);
            service["example.addVectors"] = (Func<Vector, Vector, Vector>)addVectors;

            Console.Read();
       }

        private void HandleNewClient(Connection connection)
        {
            Console.WriteLine("New Client connected!");
            connection.LoadLocalIDL("server.sinfoni");
        }

        private Vector addVectors(Vector a, Vector b)
        {
            return new Vector
            {
                x = a.x + b.x,
                y = a.y + b.y,
                z = a.z + b.z
            };
        }
    }
}
