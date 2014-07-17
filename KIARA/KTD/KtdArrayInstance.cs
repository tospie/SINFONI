using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class KtdArrayInstance : KtdTypeInstance
    {
        public KtdArrayInstance(KtdTypeInstance[] values)
        {
            Values = values;
        }

        public KtdTypeInstance[] Values { get; internal set; }

        public object AssignToLocalType(Type localType)
        {
            object[] localTypeArray = new object[Values.Length];
            for (int i = 0; i < localTypeArray.Length; i++)
            {
                localTypeArray[i] = Values[i].AssignToLocalType(localType);
            }
            return localTypeArray;
        }
    }
}
