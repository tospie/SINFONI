using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionalityTestProtocol
{
    internal class FuncTestMessage
    {
        internal string functionName { get; set; }
        internal Dictionary<string, object> parameters { get; set; }
    }
}
