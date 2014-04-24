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
        internal enum ParseMode
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
            lineNumberParsed = 0;
            currentlyParsing = ParseMode.NONE;

            string[] idlLines =
                idlString.Split(new string[] { Environment.NewLine },StringSplitOptions.RemoveEmptyEntries);

            foreach(string line in idlLines)
            {
                ++lineNumberParsed;
                parseLine(line.Trim());
            }
        }

        internal void parseLine(string line)
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
                StructParser.Instance.parseLineOfStruct(line);
            }
            else if (currentlyParsing == ParseMode.SERVICE)
            {
                ServiceParser.Instance.parseLineOfService(line, lineNumberParsed);
            }
        }

        private void startObjectParsing(string line)
        {
            if (line.Contains("struct") && line.IndexOf("struct") == 0)
            {
                currentlyParsing = ParseMode.STRUCT;
                StructParser.Instance.startStructParsing(line);
            }
            else if (line.Contains("service") && line.IndexOf("service") == 0)
            {
                currentlyParsing = ParseMode.SERVICE;
                ServiceParser.Instance.startServiceParsing(line);
            }
            else
            {
                throw new IDLParseException(line, lineNumberParsed);
            }
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

        internal ParseMode currentlyParsing = ParseMode.NONE;
        ServiceFunctionDescription currentlyParsedService;
        int lineNumberParsed = 0;
    }
}
