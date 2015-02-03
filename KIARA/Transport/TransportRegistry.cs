using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class TransportRegistry
    {
        public readonly static TransportRegistry Instance = new TransportRegistry();

        public void RegisterTransport(ITransport transport)
        {
            if (transport.Name == null)
                throw new Error(ErrorCode.INVALID_VALUE, "Transport name must not be null.");

            if (IsRegistered(transport.Name))
                throw new Error(ErrorCode.INVALID_VALUE, "Protocol " + transport.Name + " is already registered.");

            registeredTransports[transport.Name] = transport;
        }

        public ITransport GetTransport(string transportName)
        {
            if (IsRegistered(transportName))
                return registeredTransports[transportName];
            throw new Error(ErrorCode.GENERIC_ERROR, "Protocol " + transportName + " is not registered.");
        }

        public bool IsRegistered(string transportName)
        {
            return registeredTransports.ContainsKey(transportName);
        }

        private Dictionary<string, ITransport> registeredTransports = new Dictionary<string, ITransport>();
    }
}
