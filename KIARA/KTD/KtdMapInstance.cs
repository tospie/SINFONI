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

        public object AssignToLocalType(Type localType)
        {
            var keyValueTypes = localType.GetGenericArguments();
            Dictionary<object, object> localTypeDictionary = new Dictionary<object, object>();
            foreach(KeyValuePair<KtdTypeInstance, KtdTypeInstance> entry in Values)
            {
                localTypeDictionary.Add(
                        entry.Key.AssignToLocalType(keyValueTypes[0]),
                        entry.Value.AssignToLocalType(keyValueTypes[1])
                    );
            }
            return localTypeDictionary;
        }
    }
}
