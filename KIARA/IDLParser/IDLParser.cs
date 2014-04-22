using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KIARA
{
    public class IDLParser
    {
        public static IDLParser Instance = new IDLParser();

        const string idlString = @"struct AttributeDef {
                            string Guid;
                            // Strange Name
                            string Name;
                            any DefaultValue; // Some stuff
                            string Type;  // contains AssemblyQualifiedName of the type
                        }

                        struct ComponentDef {
                            string Guid;
                            string Name;
                            array<AttributeDef> AttributeDefs;
                        }

                        struct AttributeSyncInfo {
                            i64 LastTimestamp;
                            string LastSyncID;
                            any LastValue;
                        }

                        struct ComponentSyncInfo {
                            map<string, AttributeSyncInfo> Attributes;
                        }

                        struct EntitySyncInfo {
                            map<string, ComponentSyncInfo> Components;
                        }
                        ";

        internal void parseIDL(string idlString = idlString)
        {

            string trimmedIdlString = trimIdlString(idlString);
            string[] structureDefinitions = getStructureDefinitions(trimmedIdlString);
            foreach (string struc in structureDefinitions)
            {
                StructParser.Instance.parseStruct(struc);
            }
        }

        private string[] getStructureDefinitions(string idlString)
        {
            Regex structRegex = new Regex("[struct [A-Za-z0-9_]* {[A-Za-z0-9_<,>;/\n ]*}[\n ]*]*");
            MatchCollection matches = structRegex.Matches(idlString);            
            var results = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                results[i] = matches[i].Value;
            }

            return results;
        }

        private string trimIdlString(string idlString)
        {
            idlString = removeTabsAndNewlines(idlString);
            idlString = collapseWhitespaces(idlString);
            return idlString;
        }

        private string removeTabsAndNewlines(string idlString)
        {
            string escapedString = Regex.Replace(idlString, @"\t|\r", "");
            return escapedString;
        }

        private string collapseWhitespaces(string idlString)
        {
            Regex regex = new Regex(@"[ ]{2,}");
            string collapsed = regex.Replace(idlString, @" ");
            return collapsed;
        }
    }
}
