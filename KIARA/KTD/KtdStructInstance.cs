using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class KtdStructInstance : KtdTypeInstance
    {
        public KtdStructInstance(KtdType type, Dictionary<string, KtdTypeInstance> fields)
        {
            TypeDefinition = type;
            fieldValues = fields;
        }

        public KtdType TypeDefinition { get; private set; }
        public Dictionary<string, KtdTypeInstance> Fields
        {
            get { return fieldValues; }
            set { fieldValues = value; }
        }
        private Dictionary<string, KtdTypeInstance> fieldValues
            = new Dictionary<string, KtdTypeInstance>();
    }
}
