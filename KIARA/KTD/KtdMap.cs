using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("SINFONIUnitTests")]

namespace SINFONI
{
    /// <summary>
    /// Represents a map from SinTD Types to SinTDTypes. SinTDMap does not need a name and is not registered to the SinTD,
    /// but is used as member object for SinTD Structs.
    /// </summary>
    public class SinTDMap : SinTDType
    {
        public SinTDMap() { }

        public SinTDMap(SinTDType key, SinTDType element)
        {
            keyType = key;
            elementType = element;
        }

        public override bool CanBeAssignedFromType(Type type)
        {
            if (!typeof(IDictionary).IsAssignableFrom(type))
                return false;

            Type[] keyAndValueTypes = type.GetGenericArguments();

            return keyType.CanBeAssignedFromType(keyAndValueTypes[0])
                && elementType.CanBeAssignedFromType(keyAndValueTypes[1]);
        }

        public override object AssignValuesFromObject(object other)
        {
            var instanceValues = new Dictionary<object, object>();
            var otherAsDictionary = other as IDictionary;

            foreach (var key in otherAsDictionary.Keys)
            {
                var value = otherAsDictionary[key];
                object keyInstance = this.keyType.AssignValuesFromObject(key);
                object valueInstance = this.elementType.AssignValuesFromObject(value);
                instanceValues.Add(keyInstance, valueInstance);
            }

            return instanceValues;
        }

        public override object AssignValuesToNativeType(object value, Type nativeType)
        {
            if(!CanBeAssignedFromType(value.GetType()))
                throw new SINFONI.Exceptions.TypeCastException("Cannot assign value of SinTDMap to native object of type " + nativeType);

           // Dictionary<object, object> valueDictionary = (Dictionary<object, object>)value;
            Type[] keyAndValueTypes = nativeType.GetGenericArguments();
            var genericAssignDictionary = typeof(SinTDMap).GetMethod("AssignValuesToTypesDictionary");
            var genericCall = genericAssignDictionary.MakeGenericMethod(keyAndValueTypes[0], keyAndValueTypes[1]);
            return genericCall.Invoke(this, new object[] { value });
        }

        public Dictionary<TKey, TValue> AssignValuesToTypesDictionary
            <TKey, TValue>(IDictionary valueDictionary)
        {
            Dictionary<TKey, TValue> localTypeDictionary = new Dictionary<TKey, TValue>();
            foreach(var key in valueDictionary.Keys)
            {
                localTypeDictionary.Add(
                       (TKey) keyType.AssignValuesToNativeType(key, typeof(TKey)),
                       (TValue)elementType.AssignValuesToNativeType(valueDictionary[key], typeof(TValue))
                    );
            }
            return localTypeDictionary;
        }

        internal SinTDType keyType;
        internal SinTDType elementType;
    }
}
