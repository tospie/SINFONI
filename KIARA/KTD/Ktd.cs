using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class KTD
    {
        public static KTD Instance = new KTD();

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

        private Dictionary<string, KtdType> registeredTypes = new Dictionary<string,KtdType>();
    }
}
