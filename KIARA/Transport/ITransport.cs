﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public interface ITransport
    {
        string Name { get; }

        ITransportConnectionFactory TransportConnectionFactory { get; }

        ITransportAddress CreateAddress(string uri);

        void CloseConnection();
        void Send(object message);
    }
}
