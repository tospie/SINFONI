using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class ServiceFunctionDescription
    {
        public string Name { get; internal set; }
        public KtdType ReturnType { get; internal set; }

        internal ServiceFunctionDescription(string name, KtdType returnType)
        {
            Name = name;
            ReturnType = returnType;
        }

        internal Dictionary<string, KtdType> parameters =
            new Dictionary<string,KtdType>();
    }
}
