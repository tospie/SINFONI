using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using KIARA;
using WebSocketJSON;

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

            WSJConnectionFactory connectionFactory = new WSJConnectionFactory();
            ProtocolRegistry.Instance.RegisterConnectionFactory("websocket-json", connectionFactory);
            
            IServiceImpl service = ServiceFactory.Create(configURI);
            service.OnNewClient += new NewClient(HandleNewClient);

            service["example.addVectors"] = (Func<Vector, Vector, Vector>)addVectors;

            ServiceRegistry.Instance.ContainsService("dummy");
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
