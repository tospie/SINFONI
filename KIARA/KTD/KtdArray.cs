using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using KIARA.Exceptions;

[assembly: InternalsVisibleTo("KIARAUnitTests")]

namespace KIARA
{
    /// <summary>
    /// Represents an array of KTD Types. KtdArray does not need a name and is not registered to the KTD,
    /// but is used as member object for KTD Structs.
    /// </summary>
    public class KtdArray : KtdType
    {
        internal override bool canBeAssignedFromType(Type type)
        {
            // Type to match is no enumerable and can thus not be matched to an array
            if (!typeof(IEnumerable).IsAssignableFrom(type))
                return false;

            Type itemType = GetEnumerableType(type);
            
            if (itemType == null)
                return false;

            return elementType.canBeAssignedFromType(itemType);
        }

        public override object AssignValuesFromObject(object other)
        {
            if(!canBeAssignedFromType(other.GetType()))
                throw new TypeCastException("Cannot assign value to Instance of type KtdArray<" + elementType.Name + "> : "
                    + other + " is of type " + other.GetType());

            return CreateArrayInstanceFrom(other);
        }

        private KtdArrayInstance CreateArrayInstanceFrom(object other)
        {
            var array = other as IEnumerable;
            List<KtdTypeInstance> arrayValues = new List<KtdTypeInstance>();

            foreach (object value in array)
            {
                arrayValues.Add((KtdTypeInstance)elementType.AssignValuesFromObject(value));
            }

            KtdArrayInstance instance = new KtdArrayInstance(this, arrayValues.ToArray());
            return instance;
        }
        // From StackOverflow:
        // stackoverflow.com/questions/3922029/how-to-retrieve-the-generic-type-used-in-a-generic-ienumerable-in-net
        private static Type GetEnumerableType(Type type)
        {
            if (type == null) throw new ArgumentNullException();
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return interfaceType.GetGenericArguments()[0];
                }
            }
            return null;
        }

        internal KtdType elementType;
    }    
}
