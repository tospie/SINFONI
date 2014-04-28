﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA;
using KIARA.Exceptions;
using NUnit.Framework;

namespace KIARAUnitTests
{
    [TestFixture()]
    class TestIDLParser
    {
        #region ExampleIDL
        const string exampleIDL = @"struct AttributeDef {
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

        [SetUp()]
        public void SetUp()
        {
            KTD.Instance = new KTD();
            ServiceRegistry.Instance = new ServiceRegistry();
        }

        [Test()]
        public void ShouldParseCorrectArrayType()
        {
            var intArray = ArrayParser.Instance.ParseArray("array<i16>");
            Assert.AreEqual(KTD.Instance.GetKtdType("i16"), intArray.elementType);
        }

        [Test()]
        public void ShouldParseCorrectMapKeyType()
        {
            var stringToIntMap = MapParser.Instance.ParseMap("map<string,i16>");
            Assert.AreEqual(KTD.Instance.GetKtdType("string"), stringToIntMap.keyType);
        }

        [Test()]
        public void ShouldParseCorrectMapValueType()
        {
            var stringToIntMap = MapParser.Instance.ParseMap("map<string,i16>");
            Assert.AreEqual(KTD.Instance.GetKtdType("i16"), stringToIntMap.elementType);
        }

        [Test()]
        public void ShouldParseArrayDefinitionWithSpaces()
        {
            var intArray = ArrayParser.Instance.ParseArray("array < i16 >");
            Assert.AreEqual(KTD.Instance.GetKtdType("i16"), intArray.elementType);
        }

        [Test()]
        public void ShouldParseMapDefinitionsWithSpaces()
        {
            var stringToIntMap = MapParser.Instance.ParseMap("map < string, i16 >");
            Assert.AreEqual(KTD.Instance.GetKtdType("string"), stringToIntMap.keyType);
            Assert.AreEqual(KTD.Instance.GetKtdType("i16"), stringToIntMap.elementType);
        }

        [Test()]
        public void ShouldParseStructOfBaseTypes()
        {
            string idl = @"struct BaseStruct {
                            i32 intValue;
                            string name;
                         }";

            IDLParser.Instance.parseIDL(idl);
            Assert.IsTrue(KTD.Instance.ContainsType("BaseStruct"));
            KtdStruct baseStruct = KTD.Instance.GetKtdType("BaseStruct") as KtdStruct;
            Assert.Contains("intValue", baseStruct.members.Keys);
            Assert.AreEqual(KTD.Instance.GetKtdType("i32"), baseStruct.members["intValue"]);
        }

        [Test()]
        public void ShouldParseStructWithMap()
        {
            string idl = @"struct MapStruct {
                            map<string, i16> testMap;
                         }";

            IDLParser.Instance.parseIDL(idl);
            Assert.IsTrue(KTD.Instance.ContainsType("MapStruct"));
            KtdStruct mapStruct = KTD.Instance.GetKtdType("MapStruct") as KtdStruct;
            Assert.Contains("testMap", mapStruct.members.Keys);

            var parsedMap = mapStruct.members["testMap"];
            Assert.AreEqual(typeof(KtdMap), parsedMap.GetType());
        }

        [Test()]
        public void ShouldParseStructWithArray()
        {
            string idl = @"struct ArrayStruct {
                            array<i16> testArray;
                         }";
            IDLParser.Instance.parseIDL(idl);
            Assert.IsTrue(KTD.Instance.ContainsType("ArrayStruct"));
            KtdStruct arrayStruct = KTD.Instance.GetKtdType("ArrayStruct") as KtdStruct;
            Assert.Contains("testArray", arrayStruct.members.Keys);

            var parsedArray = arrayStruct.members["testArray"];
            Assert.AreEqual(typeof(KtdArray), parsedArray.GetType());
        }

        [Test()]
        public void ShouldThrowExceptionOnBadMemberDefinition()
        {
            string idl = @"struct ArrayStruct {
                            struct another struct {};
                            array<i16> testArray;
                         }";
            Assert.Throws<IDLParseException>(() => IDLParser.Instance.parseIDL(idl));
        }

        [Test()]
        public void ShouldThrowExceptionOnBadObjectDefinition()
        {
            string idl = @"struct ArrayStruct {
                            array<i16> testArray;
                         }

                         i32 MisplacedBasetype;";
            Assert.Throws<IDLParseException>(() => IDLParser.Instance.parseIDL(idl));
        }

        [Test()]
        public void ShouldParseEmptyServiceDefinition()
        {
            string idl = @"service serverSync
                            {
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.parseIDL(idl));
            Assert.Contains("serverSync", ServiceRegistry.Instance.services.Keys);
        }

        [Test()]
        public void ShouldParseServiceDefinitionWithVoidNoParams()
        {
            string idl = @"service serverSync
                            {
                                // Returns this server's sync ID.
                                void getSyncID();
                            }";
            Assert.DoesNotThrow(()=>IDLParser.Instance.parseIDL(idl));
            Assert.IsTrue(ServiceRegistry.Instance.services["serverSync"].ContainsServiceFunction("getSyncID"));

            var serviceFunction = ServiceRegistry.Instance.GetService("serverSync").GetServiceFunction("getSyncID");
            Assert.AreEqual("void", serviceFunction.ReturnType.Name);
            Assert.IsEmpty(serviceFunction.parameters);
        }

        [Test()]
        public void ShouldParseTypedServiceFunctionWithParameters()
        {
            string idl = @"service serverSync
                            {
                                string testFunction(i32 param);
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.parseIDL(idl));
            Assert.IsTrue(ServiceRegistry.Instance.services["serverSync"].ContainsServiceFunction("testFunction"));

            var serviceFunction = ServiceRegistry.Instance.GetService("serverSync").GetServiceFunction("testFunction");
            Assert.AreEqual(KTD.Instance.GetKtdType("string"), serviceFunction.ReturnType);
            Assert.Contains("param", serviceFunction.parameters.Keys);

            var parameter = serviceFunction.parameters["param"];
            Assert.AreEqual(KTD.Instance.GetKtdType("i32"), parameter);
        }

        [Test()]
        public void ShouldParseServiceFunctionWithComplexType()
        {
            string idl = @"service serverSync
                            {
                                array<string> testFunction1(i32 param);
                                map<string, boolean> testFunction2(i32 param);
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.parseIDL(idl));
            var testFunction1 = ServiceRegistry.Instance.GetService("serverSync").GetServiceFunction("testFunction1");
            var testFunction2 = ServiceRegistry.Instance.GetService("serverSync").GetServiceFunction("testFunction2");

            Assert.AreEqual(typeof(KtdArray), testFunction1.ReturnType.GetType());
            KtdArray array = (KtdArray)testFunction1.ReturnType;
            Assert.AreEqual(KTD.Instance.GetKtdType("string"), array.elementType);

            Assert.AreEqual(typeof(KtdMap), testFunction2.ReturnType.GetType());
            KtdMap map = (KtdMap)testFunction2.ReturnType;
            Assert.AreEqual(KTD.Instance.GetKtdType("string"), map.keyType);
            Assert.AreEqual(KTD.Instance.GetKtdType("boolean"), map.elementType);
        }

        [Test()]
        public void ShouldParseServiceFunctionWithComplexParameters()
        {
            string idl = @"service serverSync
                            {
                                void testFunction1(array<i32> param);
                                void testFunction2(map<string, boolean> param);
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.parseIDL(idl));

            var testFunction1 = ServiceRegistry.Instance.GetService("serverSync").GetServiceFunction("testFunction1");
            var param1 = testFunction1.parameters["param"];
            Assert.AreEqual(typeof(KtdArray), param1.GetType());
            Assert.AreEqual(KTD.Instance.GetKtdType("i32"), ((KtdArray)param1).elementType);

            var testFunction2 = ServiceRegistry.Instance.GetService("serverSync").GetServiceFunction("testFunction2");
            var param2 = testFunction2.parameters["param"];
            Assert.AreEqual(typeof(KtdMap), param2.GetType());
            Assert.AreEqual(KTD.Instance.GetKtdType("string"), ((KtdMap)param2).keyType);
            Assert.AreEqual(KTD.Instance.GetKtdType("boolean"), ((KtdMap)param2).elementType);
        }

        [Test()]
        public void ShouldParseServiceFunctionWithMultipleParameters()
        {
            string idl = @"service serverSync
                            {
                                void testFunction(string param1, array<i32> param2);                                
                            }";
            
            Assert.DoesNotThrow(() => IDLParser.Instance.parseIDL(idl));
            var testFunction = ServiceRegistry.Instance.GetService("serverSync").GetServiceFunction("testFunction");
            var param1 = testFunction.parameters["param1"];
            var param2 = testFunction.parameters["param2"];

            Assert.AreEqual(KTD.Instance.GetKtdType("string"), param1);
            Assert.AreEqual(typeof(KtdArray), param2.GetType());
            Assert.AreEqual(KTD.Instance.GetKtdType("i32"), ((KtdArray)param2).elementType);
        }

        [Test()]
        public void ShouldHandleLineComments()
        {
            string idl = @"service serverSync
                            {
                                // This is a line comment
                                void testFunction(string param1, array<i32> param2);
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.parseIDL(idl));
        }

        [Test()]
        public void ShouldHandleInlineComments()
        {
            string idl = @"service serverSync
                            {
                                void testFunction1(i32 p1 /* This is an inline comment */, string p2);
                                void testFunction2(string param1, array<i32> param2); // This is another inline comment
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.parseIDL(idl));
            Assert.IsTrue(ServiceRegistry.Instance.GetService("serverSync").ContainsServiceFunction("testFunction1"));
            var testFunction = ServiceRegistry.Instance.GetService("serverSync").GetServiceFunction("testFunction1");

            Assert.Contains("p1", testFunction.parameters.Keys);
            Assert.Contains("p2", testFunction.parameters.Keys);
        }

        [Test()]
        public void ShouldHandleBlockCommentsComments()
        {
            string idl = @"service serverSync
                            {
                                /* This is a block comment. Block comments will hopefully
                                 * be treated correctly as well */
                                void testFunction1();
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.parseIDL(idl));
            Assert.IsTrue(ServiceRegistry.Instance.GetService("serverSync").ContainsServiceFunction("testFunction1"));
        }

        [Test()]
        public void ShouldHandleBlockCommentsThatEndInline()
        {
            string idl = @"service serverSync
                            {
                                /* This is a block comment. Block comments will hopefully
                                 * be treated correctly as well  */ void testFunction1();
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.parseIDL(idl));
            Assert.IsTrue(ServiceRegistry.Instance.GetService("serverSync").ContainsServiceFunction("testFunction1"));
        }

        [Test()]
        public void ShouldParseExampleIDLWithoutError()
        {
            Assert.DoesNotThrow(() => IDLParser.Instance.parseIDL(exampleIDL));
            var services = ServiceRegistry.Instance.services;
            var types = KTD.Instance.registeredTypes;
        }
    }
}
