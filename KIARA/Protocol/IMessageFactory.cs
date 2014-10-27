using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA.Protocol
{
    public interface IMessageFactory
    {
        IMessage createCallMessage(int callID, string name, List<int> callbacks, List<object> convertedArgs);
    }
}
