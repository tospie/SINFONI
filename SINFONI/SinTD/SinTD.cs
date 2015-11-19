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
    /// <summary>
    /// SinTD (SINFONI Type Description) maintains all types registered fron a SINFONI IDL. SinTD contains all base types
    /// supported by SINFONI. During runtime, new complex types derived from these base types can be registered.
    /// </summary>
    public class SinTD
    {
        internal SinTD()
        {
            registerBaseTypes();
            SINFONIServices = new ServiceRegistry();
        }

        public ServiceRegistry SINFONIServices { get; internal set; }
        /// <summary>
        /// Checks if a type with a specific name is registered in the SinTD
        /// </summary>
        /// <param name="name">Name of the type that should be checked</param>
        /// <returns>true, if type was registered before</returns>
        public bool ContainsType(string name)
        {
            return registeredTypes.ContainsKey(name);
        }

        /// <summary>
        /// Returns a <see cref="SinTDType"/> object that is registered in the SinTD under the specified name.
        /// Throws a <see cref="TypeNotRegisteredException"/>, if a type with given name is not registered.
        /// </summary>
        /// <param name="name">Name of the type that should be returned</param>
        /// <returns></returns>
        public SinTDType GetSinTDType(string name)
        {
            if (!ContainsType(name))
            {
                throw new TypeNotRegisteredException(name);
            }

            return registeredTypes[name];
        }

        /// <summary>
        /// Registers a new Type to the SinTD. The type object that is used for registration must contain a
        /// valid name (not null, not registered before). Otherwise, SinTD will throw an exception.
        /// </summary>
        /// <param name="type">New type that should be registered to the SinTD</param>
        public void RegisterType(SinTDType type)
        {
            if(type.Name == null
                || type.Name.Length == 0)
                throw new InvalidTypeNameException();

            if (registeredTypes.ContainsKey(type.Name))
                throw new TypeNameConflictException(type.Name);

            registeredTypes.Add(type.Name, type);
        }

        private void registerBaseTypes()
        {
            RegisterType(new SinTDType("boolean", typeof(bool)));
            RegisterType(new SinTDType("byte", typeof(byte)));

            RegisterType(new SinTDType("i16", typeof(short)));
            RegisterType(new SinTDType("i32", typeof(int)));
            RegisterType(new SinTDType("i64", typeof(long)));

            RegisterType(new SinTDType("u16", typeof(ushort)));
            RegisterType(new SinTDType("u32", typeof(uint)));
            RegisterType(new SinTDType("u64", typeof(ulong)));

            RegisterType(new SinTDType("float", typeof(float)));
            RegisterType(new SinTDType("double", typeof(double)));

            RegisterType(new SinTDType("string", typeof(string)));

            RegisterType(new SinTDType("any", typeof(object)));
        }

        internal Dictionary<string, SinTDType> registeredTypes = new Dictionary<string,SinTDType>();
    }
}
