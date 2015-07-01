using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI.Exceptions
{
    public class TypeCastException : Exception
    {
        public TypeCastException(string message) : base(message) { }
    }
}
