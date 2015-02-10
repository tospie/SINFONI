using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiVESJson
{
    public class InvalidMessageTypeException : Exception
    {
        public InvalidMessageTypeException(string attemptedType) :
            base("Unknown message type for received message : " + attemptedType)
        {            
        }
    }
}
