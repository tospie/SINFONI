using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KIARA;


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
            string configURI = ServerSyncTools.ConvertFileNameToURI("server.json");

            // Connection Factory fliegt so raus -> Registered werden müssen Protokoll und Transport
            // (Register Transport / Register Protocol)

            // ServiceFactory.Create -> Start new Server / KIARA Base Server
            KIARAServer newServer = new KIARAServer("+", 8080, "/service");
            newServer.StartService("ws://0.0.0.0:34867/", "websocket", "json-rpc");

            IServiceImpl service = ServiceFactory.Create(configURI);

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
