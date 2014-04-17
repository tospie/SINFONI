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
            KtdType newStruct = new KtdType(structName);

            string[] memberDefinitions = parseMember(structDefinition);
            foreach (string memberDef in memberDefinitions)
            {
                createKtdTypeForMember(memberDef, newStruct);
            }
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

        private void createKtdTypeForMember(string memberDefinition, KtdType createdStruct)
        {
            if (!memberDefinition.Contains(';'))
                return;

            // Removes semicolon and comments at the end of line
            memberDefinition = memberDefinition.Trim().Split(';')[0];
            string[] memberComponents = memberDefinition.Split(' ');
            string memberType = memberComponents[0];
            string memberName = memberComponents[1];

            // TODO: Retrieve Types from KTD
            createdStruct.members.Add(memberName, new KtdType(memberType));
        }

    }
}
