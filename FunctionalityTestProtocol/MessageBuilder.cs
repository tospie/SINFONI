using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA;

namespace FunctionalityTestProtocol
{
    internal class MessageBuilder
    {
        internal Dictionary<string, object> CreateStructObject(string name)
        {
            return new Dictionary<string, object>();
        }

        internal void AddFieldToStruct(string name, object fieldValue, Dictionary<string, object> structParameter)
        {
            structParameter.Add(name, fieldValue);
        }

        internal void buildMessageFromParameters(ServiceFunctionDescription serviceFunction, object[] parameters)
        {
            FuncTestMessage message = new FuncTestMessage();

            var definedParameters = serviceFunction.Parameters;            

            for(var i = 0; i < parameters.Length; i++)
            {
                var IDLParam = serviceFunction.Parameters.ElementAt(i);
                message.parameters.Add(IDLParam.Key, IDLParam.Value.AssignValuesFromObject(parameters[i]));
            }
        }

        internal object WriteBaseType(object parameter)
        {
            // TODO: Correct handling of passing base type. Should be the respective write-function for the
            // type specified in the IDL.
            return parameter;
        }

    }
}
