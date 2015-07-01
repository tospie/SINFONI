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
