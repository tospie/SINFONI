using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA.Exceptions
{
    public class InvalidTypeNameException : Exception
    {
        public InvalidTypeNameException()
            : base("Failed to register type as name is either null or contains invalid characters")
        { }
    }
}
