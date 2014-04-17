using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KIARA
{
    internal class StructParser
    {
        internal void parseStruct(string structDefinition)
        {
            string structName = parseName(structDefinition);
            string[] memberDefinitions = parseMember(structDefinition);
        }

        private string parseName(string structDefinition)
        {
            Regex nameRegEx = new Regex("struct ([A-Za-z0-9_]*) [{}._<,>; ]*");
            Match nameMatch = nameRegEx.Match(structDefinition);
            string name = nameMatch.Groups[1].Value;
            return name;
        }

        private string[] parseMember(string structDefinition)
        {
            Regex memberRegEx = new Regex("[._ ]*{([A-Za-z0-9_<,>;/\n ]*)}[ ]*");
            Match memberMatch = memberRegEx.Match(structDefinition);
            string member = memberMatch.Groups[1].Value;
            return member.Split('\n');
        }
    }
}
