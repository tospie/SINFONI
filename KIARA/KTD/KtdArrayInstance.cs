using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class KtdArrayInstance : KtdTypeInstance
    {
        public KtdArrayInstance(KtdType definition, KtdTypeInstance[] values)
        {
            TypeDefinition = definition;
            Values = values;
        }
        public KtdType TypeDefinition;
        public KtdTypeInstance[] Values { get; internal set; }
    }
}
