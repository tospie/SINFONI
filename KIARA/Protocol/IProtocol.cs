using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public interface IProtocol
    {
        string MimeType { get; }
        IMessage SerializeMessage(List<object> messageParameters);
        List<object> DeserializeMessage(IMessage message);
    }
}
