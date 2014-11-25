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
        MessageType Type { get; set; }
        int ID { get; set; }
        string MethodName { get; set; }
        List<object> Parameters { get; set; }
        List<int> Callbacks { get; set;}
        object Result { get; set; }
        bool IsException { get; set; }
    }
}
