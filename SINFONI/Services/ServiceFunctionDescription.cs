using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI
{
    public class ServiceFunctionDescription
    {
        public string Name { get; internal set; }
        public SinTDType ReturnType { get; internal set; }

        public Dictionary<string, SinTDType> Parameters
        {
            get { return parameters; }
            internal set { parameters = value; }
        }

        internal bool CanBeCalledWithParameters(object[] callParameters)
        {
            if (callParameters.Length != parameters.Count)
                return false;

            for (int i = 0; i < parameters.Values.Count; i++ )
            {
                if (!parameters.Values.ElementAt(i).CanBeAssignedFromType(callParameters[i].GetType()))
                    return false;
            }
            return true;
        }

        internal bool CanBeCalledWithReturnType(Type callReturnType)
        {
            return ReturnType.CanBeAssignedFromType(callReturnType);
        }

        internal ServiceFunctionDescription(string name, SinTDType returnType)
        {
            Name = name;
            ReturnType = returnType;
        }

        private Dictionary<string, SinTDType> parameters =
            new Dictionary<string,SinTDType>();
    }
}
