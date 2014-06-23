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
        public SimpleServer()
        {
            string configURI = ServerSyncTools.ConvertFileNameToURI("server.json");

            WSJConnectionFactory connectionFactory = new WSJConnectionFactory();
            ProtocolRegistry.Instance.RegisterConnectionFactory("websocket-json", connectionFactory);
            
            Context.DefaultContext.StartServer(configURI, _ => { });
        }
    }
}
