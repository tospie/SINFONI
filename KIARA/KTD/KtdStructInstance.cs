using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public override object AssignToLocalType(Type localType)
        {
            var localTypeInstance = Activator.CreateInstance(localType);

            foreach (KeyValuePair<string, KtdTypeInstance> entry in fieldValues)
            {
                FieldInfo field = localType.GetField(entry.Key);
                if (field != null)
                {
                    var valueToSet = entry.Value.AssignToLocalType(field.FieldType);
                    field.SetValue(localTypeInstance,
                        valueToSet);
                }
                else
                {
                    PropertyInfo property = localType.GetProperty(entry.Key);
                    if (property != null)
                    {
                        property.SetValue(localTypeInstance,
                           entry.Value.AssignToLocalType(property.PropertyType),
                           null);
                    }
                }
            }
            return localTypeInstance;
        }
    }
}
