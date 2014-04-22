using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA.Exceptions;

namespace KIARA
{
    /// <summary>
    /// KTD (KIARA Type Description) maintains all types registered fron a KIARA IDL. KTD contains all base types
    /// supported by KIARA. During runtime, new complex types derived from these base types can be registered.
    /// </summary>
    public class KTD
    {
        /// <summary>
        /// Singleton handler to the Kiara Type Description
        /// </summary>
        public static KTD Instance = new KTD();

        private KTD()
        {
            registerBaseTypes();
        }

        /// <summary>
        /// Checks if a type with a specific name is registered in the KTD
        /// </summary>
        /// <param name="name">Name of the type that should be checked</param>
        /// <returns>true, if type was registered before</returns>
        public bool ContainsType(string name)
        {
            return registeredTypes.ContainsKey(name);
        }

        /// <summary>
        /// Returns a <see cref="KtdType"/> object that is registered in the KTD under the specified name.
        /// Throws a <see cref="TypeNotRegisteredException"/>, if a type with given name is not registered.
        /// </summary>
        /// <param name="name">Name of the type that should be returned</param>
        /// <returns></returns>
        public KtdType GetKtdType(string name)
        {
            if (!ContainsType(name))
            {
                throw new TypeNotRegisteredException(name);
            }

            return registeredTypes[name];
        }

        /// <summary>
        /// Registers a new Type to the Ktd. The type object that is used for registration must contain a
        /// valid name (not null, not registered before). Otherwise, KTD will throw an exception.
        /// </summary>
        /// <param name="type">New type that should be registered to the KTD</param>
        public void RegisterType(KtdType type)
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
            RegisterType(new KtdType("boolean"));
            RegisterType(new KtdType("i16"));
            RegisterType(new KtdType("i32"));
            RegisterType(new KtdType("i64"));

            RegisterType(new KtdType("u16"));
            RegisterType(new KtdType("u32"));
            RegisterType(new KtdType("u64"));

            RegisterType(new KtdType("float"));
            RegisterType(new KtdType("double"));

            RegisterType(new KtdType("string"));

            RegisterType(new KtdType("any"));
        }
        private Dictionary<string, KtdType> registeredTypes = new Dictionary<string,KtdType>();
    }
}
