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
    }
}
