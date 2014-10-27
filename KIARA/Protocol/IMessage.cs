using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public enum MessageType
    {
        REQUEST,
        RESPONSE,
        EXCEPTION
    }

    public interface IMessage
    {
        MessageType Type { get; }
        int ID { get; }
        string MethodName { get; }
        List<object> Parameters { get; }
        List<int> Callbacks { get; }
        object Result { get; }
        bool IsException { get; }
    }
}
