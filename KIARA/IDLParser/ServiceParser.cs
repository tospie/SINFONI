using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KIARA
{
    internal class ServiceParser
    {
        internal string parseName(string serviceDefinition)
        {
            Regex nameRegEx = new Regex("struct ([A-Za-z0-9_]*) [{}._<,>; ]*");
            Match nameMatch = nameRegEx.Match(serviceDefinition);
            string name = nameMatch.Groups[1].Value;
            return name;
        }

    }
}
