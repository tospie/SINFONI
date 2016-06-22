// This file is part of SINFONI.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library.  If not, see <http://www.gnu.org/licenses/>.
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

        internal SinTDType keyType { get; private set; }
        internal SinTDType elementType { get; private set; }

        public SinTDMap(SinTDType key, SinTDType element)
        {
            keyType = key;
            elementType = element;
            Type d = typeof(Dictionary<,>);
            Type[] typeArgs = {key.InstanceType, element.InstanceType};
            this.InstanceType = d.MakeGenericType(typeArgs);
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
                throw new SINFONI.Exceptions.TypeCastException("Cannot assign value of SinTDMap<"+
                    keyType.Name + ", " + elementType.Name + "> to native object of type " + nativeType);

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
    }
}
