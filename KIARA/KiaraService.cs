using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KIARA
{
    public class KiaraService
    {
        public string Name { get; internal set; }

        public KiaraService(string name)
        {
            Name = name;
        }

        internal Dictionary<string, ServiceFunctionDescription> serviceFunctions =
            new Dictionary<string, ServiceFunctionDescription>();
    }
}
