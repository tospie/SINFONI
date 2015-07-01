using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using SINFONI.Exceptions;

[assembly: InternalsVisibleTo("KIARAUnitTests")]

namespace SINFONI
{
    /// <summary>
    /// Represents an array of SinTD Types. SinTDArray does not need a name and is not registered to the SinTD,
    /// but is used as member object for SinTD Structs.
    /// </summary>
    public class SinTDArray : SinTDType
    {
        public SinTDArray() { }

        public SinTDArray(SinTDType type)
        {
            elementType = type;
        }

        public override bool CanBeAssignedFromType(Type type)
        {
            // Type to match is no enumerable and can thus not be matched to an array
            if (!typeof(IEnumerable).IsAssignableFrom(type))
                return false;

            Type itemType = GetEnumerableType(type);
            
            if (itemType == null)
                return false;

            return elementType.CanBeAssignedFromType(itemType);
        }

        public override object AssignValuesFromObject(object other)
        {
            if(!CanBeAssignedFromType(other.GetType()))
                throw new TypeCastException("Cannot assign value to Instance of type SinTDArray<" + elementType.Name + "> : "
                    + other + " is of type " + other.GetType());

            return CreateArrayInstanceFrom(other);
        }

        public override object AssignValuesToNativeType(object value, Type nativeType)
        {
            IEnumerable enumerable = value as IEnumerable;
            Type enumerableElementType = GetEnumerableType(nativeType);

            if (typeof(Array).IsAssignableFrom(nativeType))
            {
                var genericAssignArray = typeof(SinTDArray).GetMethod("AssignValuesToNativeArray");
                var genericCall = genericAssignArray.MakeGenericMethod(enumerableElementType);
                return genericCall.Invoke(this, new object[] { enumerable });
            }

            else if (typeof(IList).IsAssignableFrom(nativeType))
            {
                var genericAssignList = typeof(SinTDArray).GetMethod("AssignValuesToNativeList");
                var genericCall = genericAssignList.MakeGenericMethod(enumerableElementType);
                return genericCall.Invoke(this, new object[] { enumerable });
            }

            else
            {
                var genericAssignSet = typeof(SinTDArray).GetMethod("AssignValuesToNativeSet");
                var genericCall = genericAssignSet.MakeGenericMethod(enumerableElementType);
                return genericCall.Invoke(this, new object[] { enumerable });
            }
        }

        public T[] AssignValuesToNativeArray<T>(IEnumerable enumerable)
        {
            List<T> valueList = AssignValuesToNativeList<T>(enumerable);
            return valueList.ToArray();
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

        internal SinTDType elementType;
    }    
}
