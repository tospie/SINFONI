using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public KtdType GetType(string name)
        {
            if (!ContainsType(name))
            {
                throw new Exception("KTD does not contain Type Description with name " + name);
            }

            return registeredTypes[name];
        }

        public void RegisterType(string name, KtdType type)
        {
            registeredTypes.Add(name, type);
        }

        private void registerBaseTypes()
        {
            RegisterType("boolean", new KtdType("boolean"));
            RegisterType("i16", new KtdType("i16"));
            RegisterType("i32", new KtdType("i32"));
            RegisterType("i64", new KtdType("i64"));

            RegisterType("u16", new KtdType("u16"));
            RegisterType("u32", new KtdType("u32"));
            RegisterType("u64", new KtdType("u64"));

            RegisterType("float", new KtdType("float"));
            RegisterType("double", new KtdType("double"));

            RegisterType("string", new KtdType("string"));

            RegisterType("any", new KtdType("any"));
        }
        private Dictionary<string, KtdType> registeredTypes = new Dictionary<string,KtdType>();
    }
}
