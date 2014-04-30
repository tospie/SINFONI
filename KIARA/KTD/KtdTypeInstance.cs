using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class KtdTypeInstance
    {
        public KtdTypeInstance() { }

        public KtdTypeInstance(KtdType type, object value)
        {
            TypeDefinition = type;
            Value = value;
        }

        public KtdType TypeDefinition { get; internal set; }
        public object Value { get; internal set; }
    }
}
