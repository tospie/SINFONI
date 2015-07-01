using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SINFONI.Exceptions;

namespace SINFONI
{
    public class ServiceRegistry
    {
        public SINFONIService GetService(string name)
        {
            if (!services.ContainsKey(name))
                throw new ServiceNotRegisteredException(name);

            return services[name];
        }

        public bool ContainsService(string name)
        {
            return services.ContainsKey(name);
        }

        internal Dictionary<string, SINFONIService> services =
            new Dictionary<string, SINFONIService>();
    }
}
