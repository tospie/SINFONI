using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA.Exceptions;

namespace KIARA
{
    public class KTD
    {
        public static KTD Instance = new KTD();

        private KTD()
        {
            registerBaseTypes();
        }

        public bool ContainsType(string name)
        {
            return registeredTypes.ContainsKey(name);
        }

        public KtdType GetKtdType(string name)
        {
            if (!ContainsType(name))
            {
                throw new TypeNotRegisteredException(name);
            }

            return registeredTypes[name];
        }

        public void RegisterType(KtdType type)
        {
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
