using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class TransportRegistry
    {
        public readonly static TransportRegistry Instance = new TransportRegistry();

        public void RegisterTransport(string transportName, ITransport transport)
        {
            if (transportName == null)
                throw new Error(ErrorCode.INVALID_VALUE, "Transport name must not be null.");

            if (IsRegistered(transportName))
                throw new Error(ErrorCode.INVALID_VALUE, "Protocol " + transportName + " is already registered.");

            registeredTransports[transportName] = transport;
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
