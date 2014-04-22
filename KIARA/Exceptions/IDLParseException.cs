using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA.Exceptions
{
    public class IDLParseException : Exception
    {
        public IDLParseException(string idlFragment, string fragmentType)
            : base("Failed to parse IDL Fragment as " + fragmentType + ": " + idlFragment)
        {}
    }
}
