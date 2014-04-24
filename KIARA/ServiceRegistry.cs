using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA.Exceptions;

namespace KIARA
{
    public class ServiceRegistry
    {
        public static ServiceRegistry Instance = new ServiceRegistry();

        public KiaraService GetService(string name)
        {
            if (!services.ContainsKey(name))
                throw new ServiceNotRegisteredException(name);

            return services[name];
        }

        internal Dictionary<string, KiaraService> services =
            new Dictionary<string, KiaraService>();
    }
}
