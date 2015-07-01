using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SINFONI.Exceptions
{
    public class MissingIDLException : Exception       
    {
        public MissingIDLException() : base("No IDL contents and no reference to external IDL specified in server config") {}
    }
}
