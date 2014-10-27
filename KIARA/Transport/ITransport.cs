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
        void Send(IMessage message);
    }
}
