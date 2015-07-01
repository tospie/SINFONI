using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI.Exceptions
{
    public class TypeNotRegisteredException : Exception
    {
        public TypeNotRegisteredException(string typeName)
            : base("Type with name " + typeName + " is not defined")
        {}
    }
}
