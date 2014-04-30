using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class KtdMapInstance : KtdTypeInstance
    {
        public KtdMapInstance(KtdMap type, Dictionary<KtdTypeInstance, KtdTypeInstance> values)
        {
            TypeDefinition = type;
            Values = values;
        }

        public KtdType TypeDefinition { get; private set; }
        public Dictionary<KtdTypeInstance, KtdTypeInstance> Values { get; private set; }
    }
}
