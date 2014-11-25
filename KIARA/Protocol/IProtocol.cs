using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public interface IProtocol
    {
        string MimeType { get; }
        string Name { get; }
        object SerializeMessage(IMessage message);
        IMessage DeserializeMessage(object message);
    }
}
