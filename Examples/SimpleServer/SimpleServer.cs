using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KIARA;
using KIARA.Transport.WebSocketTransport;
using KIARA.Protocols.JsonRPC;


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
            // string configURI = ServerSyncTools.ConvertFileNameToURI("server.json");

            ITransport websocketTransport = new WebSocketTransport();
            TransportRegistry.Instance.RegisterTransport("ws", websocketTransport);

            IProtocol jsonRpc = new JsonRpcProtocol();
            ProtocolRegistry.Instance.RegisterProtocol("jsonrpc", jsonRpc);
            // Connection Factory fliegt so raus -> Registered werden müssen Protokoll und Transport
            // (Register Transport / Register Protocol)

            // ServiceFactory.Create -> Start new Server / KIARA Base Server
            KIARAServer newServer = new KIARAServer("127.0.0.1", 8080, "/service/", "server.kiara");
            var service = newServer.StartService("127.0.0.1", 44444, "/service", "ws", "jsonrpc");

            // Service erstellen: new Service(transport, protocol, url)

            service.OnNewClient += new NewClient(HandleNewClient);

            // Bleibt
            service["example.addVectors"] = (Func<Vector, Vector, Vector>)addVectors;

            // Service muss auf server registriert werden
            // KIARAServer.AddService(service)

            Console.Read();
       }

        private void HandleNewClient(Connection connection)
        {
            Console.WriteLine("New Client connected!");
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
