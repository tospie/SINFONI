using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpJSONProtocol;
using KIARA;
using WebSocketJSON;

namespace SimpleClient
{
    public class SimpleClient
    {
        public SimpleClient()
        {
            WSJConnectionFactory wsjConnectionFactory = new WSJConnectionFactory();
            HttpJSONConnectionFactory httpConnectionFactory = new HttpJSONConnectionFactory();

            ProtocolRegistry.Instance.RegisterConnectionFactory("websocket-json", wsjConnectionFactory);
            ProtocolRegistry.Instance.RegisterConnectionFactory("http-json", httpConnectionFactory);

            RemoteService = ServiceFactory.Discover(ServerSyncTools.ConvertFileNameToURI("server.json"));
            RemoteService.OnConnected += new Connected(HandleConnected);

            Console.ReadKey();
        }

        private void HandleConnected(Connection connection)
        {
            string idlURI = ServerSyncTools.ConvertFileNameToURI(RemoteService.Context.ServerConfiguarion.idlURL);
            connection.LoadIDL(idlURI);
            AddVectors = connection["example.addVectors"];
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

        IServiceWrapper RemoteService;
        ClientFunction AddVectors;
    }
}
