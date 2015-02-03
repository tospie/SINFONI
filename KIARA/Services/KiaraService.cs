using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA.Exceptions;

namespace KIARA
{
    public class KiaraService
    {
        public string Name { get; internal set; }

        public Context Context { get; private set; }

        public Delegate this[string name]
        {
            set { throw new NotImplementedException(); }
        }

        internal KiaraService(string name)
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
