using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using KIARA.Exceptions;

[assembly: InternalsVisibleTo("KIARAUnitTests")]

namespace KIARA
{
    /// <summary>
    /// Represents a KIARA Type. A KIARA Type is usually defined in an IDL file. KIARA Types include the base types
    /// supported by KIARA, as well as complex types array, map and string. A KIARA Type provides the necessary methods
    /// to check if a native type or datastructure can be mapped to the respective KIARA type
    /// </summary>
    public class KtdType
    { 
        public delegate object MappingFunction(object other);

        /// <summary>
        /// Standard Constructor
        /// </summary>
        public KtdType() { }

        /// <summary>
        /// Constructor for named types that should be registered to the KIARA Type Description (KTD)
        /// </summary>
        /// <param name="name">Name of the type</param>
        public KtdType(string name)
        {
            Name = name;
            InstanceType = typeof(KtdTypeInstance);
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
            if(!canBeAssignedFromType(other.GetType()))
                throw new TypeCastException("Cannot assign value to KtdInstance of type " + Name + ": "
                    + other + " is of Type " + other.GetType());
            return other;
        }

        }

        /// <summary>
        /// Checks if the KIARA Type can be implictly casted from a native C# type or in the case of complex types
        /// from a native data structure.
        /// </summary>
        /// <param name="type">Native type or datastructure that should be assigned to the KIARA type</param>
        /// <returns>true, if there exists an implicit cast from native type to KIARA type</returns>
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
            }

            return false;
        }

        private Type BaseType;
    }
 
}
