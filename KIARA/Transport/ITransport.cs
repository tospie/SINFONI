using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public interface ITransport
    {
        public string Name { public get; private set; }
        public ITransportAddress CreateAddress(string uri);
    }
}
