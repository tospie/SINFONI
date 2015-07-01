using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SINFONI.Transport.WebSocketTransport
{
    public class WebSocketTransport : ITransport
    {
        public string Name
        {
            get { return "ws"; }
        }

        public ITransportConnectionFactory TransportConnectionFactory
        {
            get { return WSConnectionFactory; }
        }

        public ITransportAddress CreateAddress(string uri)
        {
            throw new NotImplementedException();
        }

        WebSocketConnectionFactory WSConnectionFactory = new WebSocketConnectionFactory();
    }
}
