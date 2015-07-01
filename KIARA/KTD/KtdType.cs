using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using SINFONI.Exceptions;

[assembly: InternalsVisibleTo("SINFONIUnitTests")]

namespace SINFONI
{
    /// <summary>
    /// Represents a SINFONI Type. A SINFONI Type is usually defined in an IDL file. SINFONI Types include the base types
    /// supported by SINFONI, as well as complex types array, map and string. A SINFONI Type provides the necessary methods
    /// to check if a native type or datastructure can be mapped to the respective SINFONI type
    /// </summary>
    public class KtdType
    { 
        public delegate object MappingFunction(object other);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public KtdType() { }

        /// <summary>
        /// Constructor for named types that should be registered to the SINFONI Type Description (KTD)
        /// </summary>
        /// <param name="name">Name of the type</param>
        public KtdType(string name)
        {
            Name = name;            
        }

        internal KtdType(string name, Type baseType)
        {
            this.BaseType = baseType;
        }

        /// <summary>
        /// Name of the type
        /// </summary>
        public string Name { get; internal set; }

        public Type InstanceType { get; internal set; }

        /// <summary>
        /// Assign values from a native C# object to a KTD Type. Values are mapped by implicit cast for base types,
        /// arrays, and maps. For structs, values are mapped by name and type, or by a provided mapping function.
        /// Will throw exception when value cannot be assigned.
        /// </summary>
        /// <param name="other">C# object the values of which should be assigned to the KTD type</param>
        /// <returns>Object that corresponds to an instance of the KTD Type that maps to the C# object</returns>
        public virtual object AssignValuesFromObject(object other)
        {
            if(!CanBeAssignedFromType(other.GetType()))
                throw new TypeCastException("Cannot assign value to KtdInstance of type " + Name + ": "
                    + other + " is of Type " + other.GetType());
            return other;
        }

        /// <summary>
        /// Assigns a value from SINFONI Type to a native C# type. For base types, we do not need to do much more than
        /// bypassing the value to the actual native type, as C# takes care of the cast.
        /// </summary>
        /// <param name="value">Value to be assigned to base type</param>
        /// <param name="localType">Native C# type to which the value should be assigned</param>
        /// <returns>The KTD instance casted to Native C# type</returns>
        public virtual object AssignValuesToNativeType(object value, Type localType)
        {
            if (localType == typeof(object))
                return value;
            return Convert.ChangeType(value, localType);
        }

        /// <summary>
        /// Checks if the SINFONI Type can be implictly casted from a native C# type or in the case of complex types
        /// from a native data structure.
        /// </summary>
        /// <param name="type">Native type or datastructure that should be assigned to the SINFONI type</param>
        /// <returns>true, if there exists an implicit cast from native type to SINFONI type</returns>
        public virtual bool CanBeAssignedFromType(Type type)
        {
            switch(Name)
            {
                case "boolean": return type.IsAssignableFrom(typeof(System.Boolean));
                case "byte": return type.IsAssignableFrom(typeof(byte));
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
            }

            return false;
        }

        private Type BaseType;
    }
 
}
