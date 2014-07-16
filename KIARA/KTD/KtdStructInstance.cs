using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class KtdStructInstance : KtdTypeInstance
    {
        public KtdStructInstance(Dictionary<string, KtdTypeInstance> fields)
        {
            fieldValues = fields;
        }

        public Dictionary<string, KtdTypeInstance> Fields
        {
            get { return fieldValues; }
            set { fieldValues = value; }
        }
        private Dictionary<string, KtdTypeInstance> fieldValues
            = new Dictionary<string, KtdTypeInstance>();
    }
}
