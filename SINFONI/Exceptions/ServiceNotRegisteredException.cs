using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI.Exceptions
{
    class ServiceNotRegisteredException : Exception
    {
        public ServiceNotRegisteredException(string serviceName)
            : base("Service or ServiceFunction with name " + serviceName + " is not registered")
        { }
    }
}
