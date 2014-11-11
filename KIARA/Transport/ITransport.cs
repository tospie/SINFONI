using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public interface ITransport
    {
        string Name { get; }
        ITransportAddress CreateAddress(string uri);
        public ITransportConnection OpenConnection(string uri);
        public void CloseConnection();
        void Send(object message);
    }
}
