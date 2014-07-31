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
        public KtdArray() { }

        public KtdArray(KtdType type)
        {
            elementType = type;
        }

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

        public override object AssignValuesToNativeType(object value, Type nativeType)
        {
            IEnumerable enumerable = value as IEnumerable;
            Type enumerableElementType = GetEnumerableType(nativeType);

            if (typeof(Array).IsAssignableFrom(nativeType))
            {
                var genericAssignArray = typeof(KtdArray).GetMethod("AssignValuesToNativeArray");
                var genericCall = genericAssignArray.MakeGenericMethod(enumerableElementType);
                return genericCall.Invoke(this, new object[] { enumerable });
            }

            else if (typeof(IList).IsAssignableFrom(nativeType))
            {
                var genericAssignList = typeof(KtdArray).GetMethod("AssignValuesToNativeList");
                var genericCall = genericAssignList.MakeGenericMethod(enumerableElementType);
                return genericCall.Invoke(this, new object[] { enumerable });
            }

            else
            {
                var genericAssignSet = typeof(KtdArray).GetMethod("AssignValuesToNativeSet");
                var genericCall = genericAssignSet.MakeGenericMethod(enumerableElementType);
                return genericCall.Invoke(this, new object[] { enumerable });
            }
        }

        public T[] AssignValuesToNativeArray<T>(IEnumerable enumerable)
        {
            ISet<T> valueSet = AssignValuesToNativeSet<T>(enumerable);
            return valueSet.ToArray();
        }

        public List<T> AssignValuesToNativeList<T>(IEnumerable enumerable)
        {
            List<T> valueList = new List<T>();
            foreach (var el in enumerable)
            {
                valueList.Add((T)elementType.AssignValuesToNativeType(el, typeof(T)));
            }
            return valueList;
        }

        public ISet<T> AssignValuesToNativeSet<T>(IEnumerable enumerable)
        {
            ISet<T> valueSet = new HashSet<T>();
            foreach (var el in enumerable)
            {
                valueSet.Add((T)elementType.AssignValuesToNativeType(el, typeof(T)));
            }
            return valueSet;
        }

        private object CreateArrayInstanceFrom(object other)
        {
            var array = other as IEnumerable;
            List<object> arrayValues = new List<object>();

            foreach (object value in array)
            {
                arrayValues.Add(elementType.AssignValuesFromObject(value));
            }

            return arrayValues.ToArray();
        }

        private ISet<T> AssignValuesToSet<T>(object value)
        {
            ISet<T> returnedSet = new HashSet<T>();
            return returnedSet;
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
