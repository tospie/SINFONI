using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KIARA
{
    public class KtdStruct : KtdType
    {
        public KtdStruct(string name) : base(name) {
            InstanceType = typeof(KtdStructInstance);
        }

        public override object AssignValuesFromObject(object other)
        {
            if (!mappings.ContainsKey(other.GetType()))
            {
                mappings.Add(other.GetType(), (MappingFunction)delegate(object other2)
                {
                    return this.MapByName(other2);
                });
            }

            if (canBeAssignedFromType(other.GetType()))
            {
                return MapByName(other);
            }
            else /* if valid mapping is declared */
            {
                // perform mapping as declared in the mapping function
            }

            MappingFunction map = mappings[other.GetType()] as MappingFunction;
            map(other);

            return new Dictionary<string,object>();
        }

        object MapByName(object other)
        {
            var assignedMembers = new Dictionary<string, object>();

            foreach (KeyValuePair<string, KtdType> field in members)
            {
                var ktdValue = getFieldValueForKtdInstance(other, field.Key, field.Value);
                assignedMembers.Add(field.Key, ktdValue);
            }
            return  assignedMembers;
        }

        private object getFieldValueForKtdInstance(object other, string fieldName, KtdType ktdType)
        {
            var assignedValue = other;
            var otherField = other.GetType().GetField(fieldName);

            if (otherField == null)
            {
                var property = other.GetType().GetProperty(fieldName);
                assignedValue = ktdType.AssignValuesFromObject(property.GetValue(other, null));
            }
            else
            {
                assignedValue = ktdType.AssignValuesFromObject(otherField.GetValue(other));
            }
            return assignedValue;
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
