using KIARA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiVESJson
{
    public class FiVESJsonProtocol : IProtocol
    {
        public string MimeType
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return "fives-json"; }
        }

        public object SerializeMessage(IMessage message)
        {
            throw new NotImplementedException();
        }

        public IMessage DeserializeMessage(object message)
        {
            throw new NotImplementedException();
        }
    }
}
