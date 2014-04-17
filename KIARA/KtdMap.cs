using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("KIARAUnitTests")]

namespace KIARA
{
    public class KtdMap : KtdType
    {
        internal override bool canBeAssignedFromType(Type type)
        {
            if (!typeof(IDictionary).IsAssignableFrom(type))
                return false;

            Type[] keyAndValueTypes = type.GetGenericArguments();

            return keyType.canBeAssignedFromType(keyAndValueTypes[0])
                && elementType.canBeAssignedFromType(keyAndValueTypes[1]);
        }

        internal KtdType keyType;
        internal KtdType elementType;
    }
}
