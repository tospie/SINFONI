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

        #region ExampleIDL
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

            string trimmedIdlString = trimIdlString(idlString);
            string[] structureDefinitions = getStructureDefinitions(trimmedIdlString);
            foreach (string struc in structureDefinitions)
            {
                StructParser.Instance.parseStruct(struc);
            }
        }

        private string[] getStructureDefinitions(string idlString)
        {
            Regex structRegex = new Regex("struct [A-Za-z0-9_]* {[A-Za-z0-9_<,>;/\n ]*}[\n ]*");
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
