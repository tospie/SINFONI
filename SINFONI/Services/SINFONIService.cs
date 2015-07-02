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
using SINFONI.Exceptions;

namespace SINFONI
{
    public class SINFONIService
    {
        public string Name { get; internal set; }

        public Context Context { get; private set; }


        internal SINFONIService(string name)
        {
            Name = name;
        }

        public bool ContainsServiceFunction(string name)
        {
            return serviceFunctions.ContainsKey(name);
        }

        public ServiceFunctionDescription GetServiceFunction(string name)
        {
            if (!ContainsServiceFunction(name))
                throw new ServiceNotRegisteredException(name);

            return serviceFunctions[name];
        }


        internal Dictionary<string, ServiceFunctionDescription> serviceFunctions =
            new Dictionary<string, ServiceFunctionDescription>();
    }
}
