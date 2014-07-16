using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class KtdMapInstance : KtdTypeInstance
    {
        public KtdMapInstance(Dictionary<KtdTypeInstance, KtdTypeInstance> values)
        {
            Values = values;
        }

        public Dictionary<KtdTypeInstance, KtdTypeInstance> Values { get; private set; }
    }
}
