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
        MessageType Type { get; internal set; }
        int ID { get; internal set; }
        string MethodName { get; internal set; }
        List<object> Parameters { get; internal set; }
        List<int> Callbacks { get; internal set; }
        object Result { get; internal set; }
        bool IsException { get; internal set; }
    }
}
