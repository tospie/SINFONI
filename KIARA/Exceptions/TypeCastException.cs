using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA.Exceptions
{
    public class TypeCastException : Exception
    {
        public TypeCastException(string message) : base(message) { }
    }
}
