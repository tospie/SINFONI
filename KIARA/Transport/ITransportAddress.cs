using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public interface ITransportAddress
    {
        ITransport Transport { get; }
        int Port { get; }
        string HostName { get; }
    }
}
