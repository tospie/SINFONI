using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KIARA
{
    public class KtdStruct : KtdType
    {
        public KtdStruct(string name) : base(name) { }

        public override object AssignValuesFromObject(object other)
        {
            var result = 0;

            mappings.Add(other.GetType(), (MappingFunction)delegate(object other2)
            {
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

        bool MapByName(object other)
        {
            other.GetType().GetMembers();

            return true;
        }

        internal override bool canBeAssignedFromType(Type type)
        {
            if (validMappings.ContainsKey(type))
                return validMappings[type];

            else
                return validMappingForTypeExists(type);
        }

        private bool validMappingForTypeExists(Type type)
        {
            var fields = type.GetFields();
            var properties = type.GetProperties();

            foreach (KeyValuePair<string, KtdType> member in members)
            {
                bool memberCanBeAssigned =
                    memberCanBeAssignedFromProperties(member, properties)
                    || memberCanBeAssignedFromFields(member, fields);

                if (!memberCanBeAssigned)
                {
                    validMappings[type] = false;
                    return false;
                }
            }

            validMappings[type] = true;
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

        internal Dictionary<string, KtdType> members = new Dictionary<string, KtdType>();
        internal Dictionary<Type, bool> validMappings = new Dictionary<Type, bool>();
        internal Dictionary<Type, Delegate> mappings = new Dictionary<Type, Delegate>();
    }
}
