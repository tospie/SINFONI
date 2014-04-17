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

            KTD.Instance.RegisterType(structName, newStruct);
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

            // TODO:
            // Consider additional spaces that may occur after comma of map definitions
            string[] memberComponents;
            string memberType;
            string memberName;

            if (!(memberDefinition.Contains("array") || memberDefinition.Contains("map")))
            {
                memberComponents = memberDefinition.Split(' ');
                memberType = memberComponents[0];
                memberName = memberComponents[1];
            }
            else
            {
                memberDefinition = Regex.Replace(memberDefinition, @"\s+", "");
                int closingBracket = memberDefinition.IndexOf('>') + 1;
                memberType = memberDefinition.Substring(0, closingBracket);
                memberName = memberDefinition.Substring(closingBracket, memberDefinition.Length - closingBracket);
            }

            KtdType typeObject = getTypeForMember(memberType);

            createdStruct.members.Add(memberName, typeObject);
        }

        private KtdType getTypeForMember(string memberType)
        {
            KtdType typeObject = new KtdType();

            if (memberIsArray(memberType))
            {
                typeObject = arrayParser.ParseArray(memberType);
            }
            else if (memberIsMap(memberType))
            {
                typeObject = mapParser.ParseMap(memberType);
            }
            else
            {
                typeObject = KTD.Instance.GetType(memberType);
            }

            return typeObject;
        }

        private bool memberIsArray(string memberType)
        {
            bool isArray = Regex.IsMatch(memberType, "array<[A-Za-z0-9_]*>");
            return isArray;
        }

        private bool memberIsMap(string memberType)
        {
            bool isMap = Regex.IsMatch(memberType, "map<[A-Za-z0-9_]*,[A-Za-z0-9_]*>");
            return isMap;
        }

        private ArrayParser arrayParser = new ArrayParser();
        private MapParser mapParser = new MapParser();
    }
}
