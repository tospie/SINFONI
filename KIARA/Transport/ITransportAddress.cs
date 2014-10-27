using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public interface ITransportAddress
    {
        public ITransport Transport { public get; private set; }
        public int Port { public get; private set; }
        public string HostName { public get; private set; }
    }
}
