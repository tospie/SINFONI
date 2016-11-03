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
using SINFONI.Protocols.JsonRPC;
using SINFONI.Transport.WebSocketTransport;

namespace SimpleClient
{
    public class SimpleClient
    {
        public SimpleClient()
        {
            IProtocol simpleBinary = new SimpleBinary.SimpleBinaryProtocol();
            ITransport webSocketTransport = new WebSocketTransport();

            ProtocolRegistry.Instance.RegisterProtocol(simpleBinary);
            TransportRegistry.Instance.RegisterTransport(webSocketTransport);
            RemoteService = ServiceFactory.Discover("http://localhost:8080/service");
            RemoteService.OnConnected += new Connected(HandleConnected);

            Console.ReadKey();
        }

        private void HandleConnected(Connection connection)
        {
            AddVectors = connection["example.addVectors"];
            connection.Closed += new EventHandler<ClosedEventArgs>((o, e) =>
            {
                Console.WriteLine("Connection was closed");
            });
            callAddVectors();
        }

        private void callAddVectors()
        {
            ClientVector a = new ClientVector { x = 1, y = 2, z = 3 };
            ClientVector b = new ClientVector { x = 10, y = 20, z = 30 };
            IClientFunctionCall addCall = AddVectors(a, b);
            addCall.OnSuccess<ClientVector>((Action<ClientVector>)onResultReturned);
            addCall.OnFailure((Action)onFailureReturned);
        }

        private void onResultReturned(ClientVector result)
        {
            Console.WriteLine("Received some Result: ["
                + result.x + ","
                + result.y + ","
                + result.z + "]");
        }

        private void onFailureReturned()
        {
            Console.WriteLine("Error");
        }

        ServiceWrapper RemoteService;
        ClientFunction AddVectors;
    }
}
