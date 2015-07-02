// This file is part of SINFONI.
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 3.0 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SINFONI;
using SINFONI.Exceptions;
using NUnit.Framework;

namespace SINFONIUnitTests
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
            IDLParser.Instance.CurrentlyParsedSinTD = new SinTD();
        }

        [Test()]
        public void ShouldParseCorrectArrayType()
        {
            var intArray = ArrayParser.Instance.ParseArray("array<i16>");
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("i16"), intArray.elementType);
        }

        [Test()]
        public void ShouldParseCorrectMapKeyType()
        {
            var stringToIntMap = MapParser.Instance.ParseMap("map<string,i16>");
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("string"), stringToIntMap.keyType);
        }

        [Test()]
        public void ShouldParseCorrectMapValueType()
        {
            var stringToIntMap = MapParser.Instance.ParseMap("map<string,i16>");
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("i16"), stringToIntMap.elementType);
        }

        [Test()]
        public void ShouldParseArrayDefinitionWithSpaces()
        {
            var intArray = ArrayParser.Instance.ParseArray("array < i16 >");
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("i16"), intArray.elementType);
        }

        [Test()]
        public void ShouldParseMapDefinitionsWithSpaces()
        {
            var stringToIntMap = MapParser.Instance.ParseMap("map < string, i16 >");
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("string"), stringToIntMap.keyType);
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("i16"), stringToIntMap.elementType);
        }

        [Test()]
        public void ShouldParseArraysOfMaps()
        {
            var arrayOfMaps = ArrayParser.Instance.ParseArray("array<map<string, i32>>");
            Assert.AreEqual(arrayOfMaps.elementType.GetType(), typeof(SinTDMap));
        }

        [Test()]
        public void ShouldParseStructOfBaseTypes()
        {
            string idl = @"struct BaseStruct {
                            i32 intValue;
                            string name;
                         }";

            IDLParser.Instance.ParseIDL(idl);
            Assert.IsTrue(IDLParser.Instance.CurrentlyParsedSinTD.ContainsType("BaseStruct"));
            SinTDStruct baseStruct = IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("BaseStruct") as SinTDStruct;
            Assert.Contains("intValue", baseStruct.members.Keys);
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("i32"), baseStruct.members["intValue"]);
        }

        [Test()]
        public void ShouldParseStructWithMap()
        {
            string idl = @"struct MapStruct {
                            map<string, i16> testMap;
                         }";

            IDLParser.Instance.ParseIDL(idl);
            Assert.IsTrue(IDLParser.Instance.CurrentlyParsedSinTD.ContainsType("MapStruct"));
            SinTDStruct mapStruct = IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("MapStruct") as SinTDStruct;
            Assert.Contains("testMap", mapStruct.members.Keys);

            var parsedMap = mapStruct.members["testMap"];
            Assert.AreEqual(typeof(SinTDMap), parsedMap.GetType());
        }

        [Test()]
        public void ShouldParseStructWithArray()
        {
            string idl = @"struct ArrayStruct {
                            array<i16> testArray;
                         }";
            IDLParser.Instance.ParseIDL(idl);
            Assert.IsTrue(IDLParser.Instance.CurrentlyParsedSinTD.ContainsType("ArrayStruct"));
            SinTDStruct arrayStruct = IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("ArrayStruct") as SinTDStruct;
            Assert.Contains("testArray", arrayStruct.members.Keys);

            var parsedArray = arrayStruct.members["testArray"];
            Assert.AreEqual(typeof(SinTDArray), parsedArray.GetType());
        }

        [Test()]
        public void ShouldThrowExceptionOnBadMemberDefinition()
        {
            string idl = @"struct ArrayStruct {
                            struct another struct {};
                            array<i16> testArray;
                         }";
            Assert.Throws<IDLParseException>(() => IDLParser.Instance.ParseIDL(idl));
        }

        [Test()]
        public void ShouldThrowExceptionOnBadObjectDefinition()
        {
            string idl = @"struct ArrayStruct {
                            array<i16> testArray;
                         }

                         i32 MisplacedBasetype;";
            Assert.Throws<IDLParseException>(() => IDLParser.Instance.ParseIDL(idl));
        }

        [Test()]
        public void ShouldParseEmptyServiceDefinition()
        {
            string idl = @"service serverSync
                            {
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.ParseIDL(idl));
            Assert.Contains("serverSync", IDLParser.Instance.CurrentlyParsedSinTD.SINFONIServices.services.Keys);
        }

        [Test()]
        public void ShouldParseServiceDefinitionWithVoidNoParams()
        {
            string idl = @"service serverSync
                            {
                                // Returns this server's sync ID.
                                void getSyncID();
                            }";
            Assert.DoesNotThrow(()=>IDLParser.Instance.ParseIDL(idl));
            Assert.IsTrue(IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.services["serverSync"].ContainsServiceFunction("getSyncID"));

            var serviceFunction = IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("getSyncID");
            Assert.AreEqual("void", serviceFunction.ReturnType.Name);
            Assert.IsEmpty(serviceFunction.Parameters);
        }

        [Test()]
        public void ShouldParseTypedServiceFunctionWithParameters()
        {
            string idl = @"service serverSync
                            {
                                string testFunction(i32 param);
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.ParseIDL(idl));
            Assert.IsTrue(IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.services["serverSync"].ContainsServiceFunction("testFunction"));

            var serviceFunction = IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction");
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("string"), serviceFunction.ReturnType);
            Assert.Contains("param", serviceFunction.Parameters.Keys);

            var parameter = serviceFunction.Parameters["param"];
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("i32"), parameter);
        }

        [Test()]
        public void ShouldParseServiceFunctionWithComplexType()
        {
            string idl = @"service serverSync
                            {
                                array<string> testFunction1(i32 param);
                                map<string, boolean> testFunction2(i32 param);
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.ParseIDL(idl));
            var testFunction1 = IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction1");
            var testFunction2 = IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction2");

            Assert.AreEqual(typeof(SinTDArray), testFunction1.ReturnType.GetType());
            SinTDArray array = (SinTDArray)testFunction1.ReturnType;
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("string"), array.elementType);

            Assert.AreEqual(typeof(SinTDMap), testFunction2.ReturnType.GetType());
            SinTDMap map = (SinTDMap)testFunction2.ReturnType;
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("string"), map.keyType);
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("boolean"), map.elementType);
        }

        [Test()]
        public void ShouldParseServiceFunctionWithComplexParameters()
        {
            string idl = @"service serverSync
                            {
                                void testFunction1(array<i32> param);
                                void testFunction2(map<string, boolean> param);
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.ParseIDL(idl));

            var testFunction1 = IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction1");
            var param1 = testFunction1.Parameters["param"];
            Assert.AreEqual(typeof(SinTDArray), param1.GetType());
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("i32"), ((SinTDArray)param1).elementType);

            var testFunction2 = IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction2");
            var param2 = testFunction2.Parameters["param"];
            Assert.AreEqual(typeof(SinTDMap), param2.GetType());
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("string"), ((SinTDMap)param2).keyType);
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("boolean"), ((SinTDMap)param2).elementType);
        }

        [Test()]
        public void ShouldParseServiceFunctionWithMultipleParameters()
        {
            string idl = @"service serverSync
                            {
                                void testFunction(string param1, array<i32> param2);                                
                            }";
            
            Assert.DoesNotThrow(() => IDLParser.Instance.ParseIDL(idl));
            var testFunction = IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction");
            var param1 = testFunction.Parameters["param1"];
            var param2 = testFunction.Parameters["param2"];

            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("string"), param1);
            Assert.AreEqual(typeof(SinTDArray), param2.GetType());
            Assert.AreEqual(IDLParser.Instance.CurrentlyParsedSinTD.GetSinTDType("i32"), ((SinTDArray)param2).elementType);
        }

        [Test()]
        public void ShouldHandleLineComments()
        {
            string idl = @"service serverSync
                            {
                                // This is a line comment
                                void testFunction(string param1, array<i32> param2);
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.ParseIDL(idl));
        }

        [Test()]
        public void ShouldHandleInlineComments()
        {
            string idl = @"service serverSync
                            {
                                void testFunction1(i32 p1 /* This is an inline comment */, string p2);
                                void testFunction2(string param1, array<i32> param2); // This is another inline comment
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.ParseIDL(idl));
            Assert.IsTrue(IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").ContainsServiceFunction("testFunction1"));
            var testFunction = IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction1");

            Assert.Contains("p1", testFunction.Parameters.Keys);
            Assert.Contains("p2", testFunction.Parameters.Keys);
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
            Assert.DoesNotThrow(() => IDLParser.Instance.ParseIDL(idl));
            Assert.IsTrue(IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").ContainsServiceFunction("testFunction1"));
        }

        [Test()]
        public void ShouldHandleBlockCommentsThatEndInline()
        {
            string idl = @"service serverSync
                            {
                                /* This is a block comment. Block comments will hopefully
                                 * be treated correctly as well  */ void testFunction1();
                            }";
            Assert.DoesNotThrow(() => IDLParser.Instance.ParseIDL(idl));
            Assert.IsTrue(IDLParser.Instance.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").ContainsServiceFunction("testFunction1"));
        }

        [Test()]
        public void ShouldParseExampleIDLWithoutError()
        {
            Assert.DoesNotThrow(() => IDLParser.Instance.ParseIDL(exampleIDL));
            var services = IDLParser.Instance.CurrentlyParsedSinTD.SINFONIServices.services;
            var types = IDLParser.Instance.CurrentlyParsedSinTD.registeredTypes;
        }
    }
}
