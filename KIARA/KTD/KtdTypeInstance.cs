using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class KtdTypeInstance
    {
        public KtdTypeInstance() { }

        public KtdTypeInstance(object value)
        {
            Value = value;
        }

        public object Value { get; internal set; }
    }
}
