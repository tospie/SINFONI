using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI.Exceptions
{
    public class ParameterMismatchException : Exception
    {
        public ParameterMismatchException(string message) : base(message) { }
    }
}
