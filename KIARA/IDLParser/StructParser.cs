using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KIARA
{
    internal class StructParser
    {
        internal static StructParser Instance = new StructParser();

        internal void startStructParsing(string structDefinition)
        {
            Regex nameRegEx = new Regex("struct ([A-Za-z0-9_]*) [{}._<,>; ]*");
            Match nameMatch = nameRegEx.Match(structDefinition);
            string name = nameMatch.Groups[1].Value;
            currentlyParsedStruct = new KtdType(name);
        }


        internal void parseLineOfStruct(string line)
        {
            // Line contains a closing bracket. In this case, struct definition is finished
            // and parsing for the current struct can be finished
            if (line.Contains('}'))
            {
                handleLastLineOfStruct(line);
            }
            else
            {
                createKtdTypeForMember(line, currentlyParsedStruct);
            }
        }

        internal void handleLastLineOfStruct(string line)
        {
            // The closing bracket may appear at the end of the actual last line of the struct definition. Treat the
            // content of the line up to the closing bracket as member definition and try to parse it
            if (line.IndexOf('}') > 0)
            {
                string lastLine = line.Split('}')[0].Trim();
                createKtdTypeForMember(lastLine, currentlyParsedStruct);
                // finalize parsing the struct after having parsed the last line.
                finalizeStructParsing();
            }
            // If the closing bracket is the first character in line, no new information is added to the struct.
            else
            {
                finalizeStructParsing();

                // If the closing bracket is the first character in the line, but there is more content after that,
                // treat the remaining content as content of a new line.
                if (line.Length > 1)
                    IDLParser.Instance.parseLine(line.Split('}')[1]);
            }
        }

        internal void finalizeStructParsing()
        {
            IDLParser.Instance.currentlyParsing = KIARA.IDLParser.ParseMode.NONE;
            KTD.Instance.RegisterType(currentlyParsedStruct);
        }


        internal void createKtdTypeForMember(string memberDefinition, KtdType createdStruct)
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
                typeObject = ArrayParser.Instance.ParseArray(memberType);
            }
            else if (memberIsMap(memberType))
            {
                typeObject = MapParser.Instance.ParseMap(memberType);
            }
            else
            {
                typeObject = KTD.Instance.GetKtdType(memberType);
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

        KtdType currentlyParsedStruct;
    }
}
