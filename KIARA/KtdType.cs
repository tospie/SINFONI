using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("KIARAUnitTests")]

namespace KIARA
{

    public class KtdType
    { 
        delegate object MappingFunction(object other);

        public KtdType() { }

        public KtdType(string name)
        {
            Name = name;
        }

        public string Name { get; internal set; }

        public object AssignValuesFromObject(object other)
        {
            var result = 0;

            mappings.Add(other.GetType(), (MappingFunction) delegate(object other2){
                return this.MapByName(other2);
            });

            if (canBeAssignedFromType(other.GetType()))
            {
                // Map object by name-type-matching
            }
            else /* if valid mapping is declared */
            {
                // perform mapping as declared in the mapping function
            }

            MappingFunction map = mappings[other.GetType()] as MappingFunction;
            map(other);

            return result;
        }

        internal virtual bool canBeAssignedFromType(Type type)
        {
            switch(Name)
            {
                case "boolean": return type.IsAssignableFrom(typeof(System.Boolean));
                case "i16": return type.IsAssignableFrom(typeof(System.Int16));                
                case "u16": return type.IsAssignableFrom(typeof(System.UInt16));
                case "i32": return type.IsAssignableFrom(typeof(System.Int32));
                case "u32": return type.IsAssignableFrom(typeof(System.UInt32));
                case "i64": return type.IsAssignableFrom(typeof(System.Int64));
                case "u64": return type.IsAssignableFrom(typeof(System.UInt64));
                case "float": return type.IsAssignableFrom(typeof(System.Single));
                case "double": return type.IsAssignableFrom(typeof(System.Double));
                case "string": return type.IsAssignableFrom(typeof(System.String));
                case "any": return true;
                default: return canBeAssignedFromComplexType(type);                
            }            
        }

        private bool canBeAssignedFromComplexType(Type type)
        {
            var fields = type.GetFields();
            var properties = type.GetProperties();

            foreach (KeyValuePair<string, KtdType> member in members)
            {
                bool memberCanBeAssigned =
                    memberCanBeAssignedFromProperties(member, properties)
                    || memberCanBeAssignedFromFields(member, fields);

                if (!memberCanBeAssigned)
                    return false;
            }
            return true;
        }

        private bool memberCanBeAssignedFromFields(KeyValuePair<string, KtdType> member, FieldInfo[] fieldInfo)
        {
            int indexOfMemberInArray = Array.FindIndex(fieldInfo,
                delegate(FieldInfo element)
                {
                    bool containsElement = element.Name.Equals(member.Key);
                    return containsElement;
                });

            if (indexOfMemberInArray == -1)
                return false;

            var field = fieldInfo[indexOfMemberInArray];

            if (!member.Value.canBeAssignedFromType(field.FieldType))
                return false;

            return true;
        }

        private bool memberCanBeAssignedFromProperties(KeyValuePair<string, KtdType> member, PropertyInfo[] propertyInfo)
        {
            int indexOfMemberInArray = Array.FindIndex(propertyInfo,
                delegate(PropertyInfo element)
                {
                    bool containsElement = element.Name.Equals(member.Key);
                    return containsElement;
                });

            if (indexOfMemberInArray == -1)
                return false;

            var property = propertyInfo[indexOfMemberInArray];

            if (!member.Value.canBeAssignedFromType(property.PropertyType))
                return false;

            return true;
        }

        bool MapByName(object other)
        {
            other.GetType().GetMembers();
            
            return true;
        }

        internal Dictionary<string, KtdType> members = new Dictionary<string,KtdType>();
        internal Dictionary<Type, Delegate> mappings = new Dictionary<Type, Delegate>();
    }
 
}
