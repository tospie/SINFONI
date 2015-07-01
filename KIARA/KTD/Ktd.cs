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
            RegisterType(new SinTDType("boolean"));
            RegisterType(new SinTDType("byte"));

            RegisterType(new SinTDType("i16"));
            RegisterType(new SinTDType("i32"));
            RegisterType(new SinTDType("i64"));

            RegisterType(new SinTDType("u16"));
            RegisterType(new SinTDType("u32"));
            RegisterType(new SinTDType("u64"));

            RegisterType(new SinTDType("float"));
            RegisterType(new SinTDType("double"));

            RegisterType(new SinTDType("string"));

            RegisterType(new SinTDType("any"));
        }

        internal Dictionary<string, SinTDType> registeredTypes = new Dictionary<string,SinTDType>();
    }
}
