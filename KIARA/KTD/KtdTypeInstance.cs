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

        public virtual object AssignToLocalType(Type localType)
        {
            var localTypeInstance = Activator.CreateInstance(localType);
            localTypeInstance = Convert.ChangeType(Value, localType);
            return localTypeInstance;
        }
    }
}
