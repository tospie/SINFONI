using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KIARA;
using KIARA.Transport.WebSocketTransport;
using KIARA.Protocols.JsonRPC;
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

            KIARAServer newServer = new KIARAServer("localhost", 8080, "/service/", "server.kiara");
            var service = newServer.StartService("127.0.0.1", 34568, "/service", "ws", "jsonrpc");

            service.OnNewClient += new NewClient(HandleNewClient);
            service["example.addVectors"] = (Func<Vector, Vector, Vector>)addVectors;

            Console.Read();
       }

        private void HandleNewClient(Connection connection)
        {
            Console.WriteLine("New Client connected!");
            connection.LoadLocalIDL("server.kiara");
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
