using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA.Exceptions;

namespace KIARA
{
    public abstract class Connection
    {
        // TODO: Change Return type to ICLientFunctionCall as soon as the calling mechanism is implemented
        public delegate void ClientFunction(params object[] parameters);

        public ClientFunction GenerateClientFunction(string serviceName, string functionName)
        {
            if (!ServiceRegistry.Instance.ContainsService(serviceName))
                throw new ServiceNotRegisteredException(serviceName);

            var service = ServiceRegistry.Instance.GetService(serviceName);

            if (!service.ContainsServiceFunction(functionName))
                throw new ServiceNotRegisteredException(functionName);

            return (ClientFunction)delegate(object[] parameters)
            {
                KiaraService registeredService = ServiceRegistry.Instance.GetService(serviceName);
                ServiceFunctionDescription registeredServiceFunction = registeredService.GetServiceFunction(functionName);

                if (!registeredServiceFunction.CanBeCalledWithParameters(parameters))
                {
                    throw new ParameterMismatchException(
                        "Could not call Service Function " + serviceName + "." + functionName
                            + ". The provided parameters can not be mapped to the parameters specified in the IDL.");
                }
                // TODO: Implement Calling remote functions
                // return CallClientFunction(serviceName, parameters);
            };
        }
    }
}
