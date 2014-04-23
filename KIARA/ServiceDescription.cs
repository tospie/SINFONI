using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class ServiceDescription
    {
        string Name { get; internal set; }
        KtdType ReturnType { get; internal set; }

        internal Dictionary<string, KtdType> parameters;
    }
}
