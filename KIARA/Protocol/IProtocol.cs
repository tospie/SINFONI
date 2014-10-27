using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public interface IProtocol
    {
        void WriteStructBegin(string structName);
        void WriteStructEnd();

        void WriteFieldBegin(string fieldName);
        void WriteFieldEnd();

        void WriteArrayBegin();
        void WriteArrayEnd();

        void WriteMapBegin();
        void WriteMapEnd();

        void WriteMessageValue<T>(object value);
    }
}
