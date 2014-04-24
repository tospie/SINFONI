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

        internal Dictionary<string, KtdType> parameters;
    }
}
