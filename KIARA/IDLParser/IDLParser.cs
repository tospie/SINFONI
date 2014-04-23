using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using KIARA.Exceptions;

namespace KIARA
{
    public class IDLParser
    {
        private enum ParseMode
        {
            COMMENT,
            STRUCT,
            SERVICE,
            NONE
        }

        public static IDLParser Instance = new IDLParser();

        #region ExampleIDL
        const string idlString = @"struct AttributeDef {
                            string Guid;
                            // Strange Name
                            string Name; /* This is some strange name assigned to the line */
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

                        service serverSync
                        {
                            // Returns this server's sync ID.
                            string getSyncID();

                            // Called to notify that an entity has been added to the caller's world. Receiving side should create a new entity
                            // if it isn't present in the world and use provided initialSyncInfo as a starting sync info. If the entity exists
                            // already, this message should be processed as changeAttributes, where each individual attribute of an entity is
                            // synchronized.
                            void addEntity(string guid, EntitySyncInfo initialSyncInfo);

                            // Called to notify that an entity has been removed from the caller's world. If the entity is not present on the
                            // receiving side, this messages should be ignored (except relay nodes, which still should relay it further).
                            // @param guid Guid of the removed entity.
                            void removeEntity(string guid);

                            // Called to notify that a set of entity's attributes has been modified in the caller's world. The receiving side
                            // should process each attribute and update local attributes where necessary.
                            void changeAttributes(string guid, EntitySyncInfo changedAttributes);

                            // Called to notify the other parties that a plugin handling given component definition has been loaded remotely.
                            // The receving side should register the component locally and remove this component from the deferred plugins'
                            // dependencies.
                            void registerComponentDefinition(ComponentDef definition);

                            // Returns serialized version of the remote server's domain-of-responsibility.
                            string getDoR();

                            // Returns serialized version of the remote server's domain-of-interest.
                            string getDoI();

                            // Called by the remote server to notify on changes to its domain-of-reponsibility, serialized version of which is
                            // passed as a parameter.
                            void updateDoI(string newDoI);

                            // Called by the remote server to notify on changes to its domain-of-interest, serialized version of which is
                            // passed as a parameter.
                            void updateDoR(string newDoR);
                        }
                        ";
        #endregion

        internal void parseIDL(string idlString = idlString)
        {
            string[] idlLines =
                idlString.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries);

            foreach(string line in idlLines)
            {
                ++lineNumberParsed;
                parseLine(line.Trim());
            }
        }

        private void parseLine(string line)
        {
            if(lineIsComment(line))
                return;

            line = removeCommentsFromLine(line);

            if (currentlyParsing == ParseMode.NONE)
            {
                startObjectParsing(line);
            }
            else if (currentlyParsing == ParseMode.STRUCT)
            {
                parseLineOfStruct(line);
            }
        }

        private void startObjectParsing(string line)
        {
            if (line.Contains("struct") && line.IndexOf("struct") == 0)
            {
                var structName = StructParser.Instance.parseName(line);
                currentlyParsedStruct = new KtdType(structName);
                currentlyParsing = ParseMode.STRUCT;
            }
            else if (line.Contains("service") && line.IndexOf("service") == 0)
            {
                currentlyParsing = ParseMode.SERVICE;
            }
            else
            {
                throw new IDLParseException(line, lineNumberParsed);
            }
        }

        private void parseLineOfStruct(string line)
        {
            // Line contains a closing bracket. In this case, struct definition is finished
            // and parsing for the current struct can be finished
            if (line.Contains('}'))
            {
                handleLastLineOfStruct(line);
            }
            else
            {
                StructParser.Instance.createKtdTypeForMember(line, currentlyParsedStruct);
            }
        }

        private void handleLastLineOfStruct(string line)
        {
            // The closing bracket may appear at the end of the actual last line of the struct definition. Treat the
            // content of the line up to the closing bracket as member definition and try to parse it
            if (line.IndexOf('}') > 0)
            {
                string lastLine = line.Split('}')[0].Trim();
                StructParser.Instance.createKtdTypeForMember(lastLine, currentlyParsedStruct);
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
                    parseLine(line.Split('}')[1]);
            }
        }

        private void finalizeStructParsing()
        {
            currentlyParsing = ParseMode.NONE;
            KTD.Instance.RegisterType(currentlyParsedStruct);
        }

        private bool lineIsComment(string line)
        {
            return currentlyParsing == ParseMode.COMMENT
                || line.Contains("//") && line.IndexOf("//") == 0
                || line.Contains("/*") && line.IndexOf("/*") == 0;
        }

        private string removeCommentsFromLine(string line)
        {
            if (line.Contains("//"))
                line = line.Substring(0, line.IndexOf("//"));

            if (line.Contains("/*"))
            {
                if (line.Contains("*/"))
                    line = line.Remove(line.IndexOf("/*"), line.IndexOf("*/") - line.IndexOf("/*") + 2);
                else
                    line = line.Substring(0, line.IndexOf("/*"));
            }

            return line.Trim();
        }

        private ParseMode currentlyParsing = ParseMode.NONE;
        KtdType currentlyParsedStruct;
        int lineNumberParsed = 0;
    }
}
