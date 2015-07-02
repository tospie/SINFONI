// This file is part of SINFONI.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI
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
