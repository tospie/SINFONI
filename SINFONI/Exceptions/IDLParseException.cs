using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI.Exceptions
{
    public class IDLParseException : Exception
    {
        public IDLParseException(string line, int lineNumber)
            : base("Cannot parse IDL. Failed at parsing line [" + lineNumber + "]: " + line)
        {}
    }
}
