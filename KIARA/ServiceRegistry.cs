using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class ServiceRegistry
    {
        public static ServiceRegistry Instance = new ServiceRegistry();

        internal Dictionary<string, KiaraService> services =
            new Dictionary<string, KiaraService>();
    }
}
