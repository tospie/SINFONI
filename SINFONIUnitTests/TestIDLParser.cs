﻿// This file is part of SINFONI.
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
        ArrayParser arrayParser;
        MapParser mapParser;
        IDLParser idlParser = new IDLParser();

        [SetUp()]
        public void SetUp()
        {
            idlParser.CurrentlyParsedSinTD = new SinTD();
            arrayParser = new ArrayParser(idlParser);
            mapParser = new MapParser(idlParser);
        }

        [Test()]
        public void ShouldParseCorrectArrayType()
        {
            var intArray = arrayParser.ParseArray("array<i16>");
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("i16"), intArray.ElementType);
        }

        [Test()]
        public void ShouldParseCorrectMapKeyType()
        {
            var stringToIntMap = mapParser.ParseMap("map<string,i16>");
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("string"), stringToIntMap.keyType);
        }

        [Test()]
        public void ShouldParseCorrectMapValueType()
        {
            var stringToIntMap = mapParser.ParseMap("map<string,i16>");
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("i16"), stringToIntMap.elementType);
        }

        [Test()]
        public void ShouldParseArrayDefinitionWithSpaces()
        {
            var intArray = arrayParser.ParseArray("array < i16 >");
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("i16"), intArray.ElementType);
        }

        [Test()]
        public void ShouldParseMapDefinitionsWithSpaces()
        {
            var stringToIntMap = mapParser.ParseMap("map < string, i16 >");
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("string"), stringToIntMap.keyType);
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("i16"), stringToIntMap.elementType);
        }

        [Test()]
        public void ShouldParseArraysOfMaps()
        {
            var arrayOfMaps = arrayParser.ParseArray("array<map<string, i32>>");
            Assert.AreEqual(arrayOfMaps.ElementType.GetType(), typeof(SinTDMap));
        }

        [Test()]
        public void ShouldParseMapOfArrays()
        {
            var mapOfArrays = mapParser.ParseMap("map<i32, array<i32>>");
            Assert.AreEqual(mapOfArrays.elementType.GetType(), typeof(SinTDArray));
        }

        [Test()]
        public void ShouldParseStructOfBaseTypes()
        {
            string idl = @"struct BaseStruct {
                            i32 intValue;
                            string name;
                         }";

            idlParser.ParseIDL(idl);
            Assert.IsTrue(idlParser.CurrentlyParsedSinTD.ContainsType("BaseStruct"));
            SinTDStruct baseStruct = idlParser.CurrentlyParsedSinTD.GetSinTDType("BaseStruct") as SinTDStruct;
            Assert.Contains("intValue", baseStruct.members.Keys);
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("i32"), baseStruct.members["intValue"]);
        }

        [Test()]
        public void ShouldParseStructWithMap()
        {
            string idl = @"struct MapStruct {
                            map<string, i16> testMap;
                         }";

            idlParser.ParseIDL(idl);
            Assert.IsTrue(idlParser.CurrentlyParsedSinTD.ContainsType("MapStruct"));
            SinTDStruct mapStruct = idlParser.CurrentlyParsedSinTD.GetSinTDType("MapStruct") as SinTDStruct;
            Assert.Contains("testMap", mapStruct.members.Keys);

            var parsedMap = mapStruct.members["testMap"];
            Assert.AreEqual(typeof(SinTDMap), parsedMap.GetType());
        }

        [Test()]
        public void ShouldParseStructWithArray()
        {
            string idl = @"struct ArrayStruct1 {
                            array<i16> testArray;
                         }";
            idlParser.ParseIDL(idl);
            Assert.IsTrue(idlParser.CurrentlyParsedSinTD.ContainsType("ArrayStruct1"));
            SinTDStruct arrayStruct = idlParser.CurrentlyParsedSinTD.GetSinTDType("ArrayStruct1") as SinTDStruct;
            Assert.Contains("testArray", arrayStruct.members.Keys);

            var parsedArray = arrayStruct.members["testArray"];
            Assert.AreEqual(typeof(SinTDArray), parsedArray.GetType());
        }

        [Test()]
        public void ShouldThrowExceptionOnBadMemberDefinition()
        {
            string idl = @"struct ArrayStruct2 {
                            struct another struct {};
                            array<i16> testArray;
                         }";
            Assert.Throws<IDLParseException>(() => idlParser.ParseIDL(idl));
        }

        [Test()]
        public void ShouldThrowExceptionOnBadObjectDefinition()
        {
            string idl = @"struct ArrayStruct3 {
                            array<i16> testArray;
                         }

                         i32 MisplacedBasetype;";
            Assert.Throws<IDLParseException>(() => idlParser.ParseIDL(idl));
        }

        [Test()]
        public void ShouldParseEmptyServiceDefinition()
        {
            string idl = @"service serverSync
                            {
                            }";
            Assert.DoesNotThrow(() => idlParser.ParseIDL(idl));
            Assert.Contains("serverSync", idlParser.CurrentlyParsedSinTD.SINFONIServices.services.Keys);
        }

        [Test()]
        public void ShouldParseServiceDefinitionWithVoidNoParams()
        {
            string idl = @"service serverSync
                            {
                                // Returns this server's sync ID.
                                void getSyncID();
                            }";
            Assert.DoesNotThrow(()=>idlParser.ParseIDL(idl));
            Assert.IsTrue(idlParser.CurrentlyParsedSinTD
                .SINFONIServices.services["serverSync"].ContainsServiceFunction("getSyncID"));

            var serviceFunction = idlParser.CurrentlyParsedSinTD
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
            Assert.DoesNotThrow(() => idlParser.ParseIDL(idl));
            Assert.IsTrue(idlParser.CurrentlyParsedSinTD
                .SINFONIServices.services["serverSync"].ContainsServiceFunction("testFunction"));

            var serviceFunction = idlParser.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction");
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("string"), serviceFunction.ReturnType);
            Assert.Contains("param", serviceFunction.Parameters.Keys);

            var parameter = serviceFunction.Parameters["param"];
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("i32"), parameter);
        }

        [Test()]
        public void ShouldParseServiceFunctionWithComplexType()
        {
            string idl = @"service serverSync
                            {
                                array<string> testFunction1(i32 param);
                                map<string, boolean> testFunction2(i32 param);
                            }";
            Assert.DoesNotThrow(() => idlParser.ParseIDL(idl));
            var testFunction1 = idlParser.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction1");
            var testFunction2 = idlParser.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction2");

            Assert.AreEqual(typeof(SinTDArray), testFunction1.ReturnType.GetType());
            SinTDArray array = (SinTDArray)testFunction1.ReturnType;
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("string"), array.ElementType);

            Assert.AreEqual(typeof(SinTDMap), testFunction2.ReturnType.GetType());
            SinTDMap map = (SinTDMap)testFunction2.ReturnType;
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("string"), map.keyType);
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("boolean"), map.elementType);
        }

        [Test()]
        public void ShouldParseServiceFunctionWithComplexParameters()
        {
            string idl = @"service serverSync
                            {
                                void testFunction1(array<i32> param);
                                void testFunction2(map<string, boolean> param);
                            }";
            Assert.DoesNotThrow(() => idlParser.ParseIDL(idl));

            var testFunction1 = idlParser.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction1");
            var param1 = testFunction1.Parameters["param"];
            Assert.AreEqual(typeof(SinTDArray), param1.GetType());
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("i32"), ((SinTDArray)param1).ElementType);

            var testFunction2 = idlParser.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction2");
            var param2 = testFunction2.Parameters["param"];
            Assert.AreEqual(typeof(SinTDMap), param2.GetType());
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("string"), ((SinTDMap)param2).keyType);
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("boolean"), ((SinTDMap)param2).elementType);
        }

        [Test()]
        public void ShouldParseServiceFunctionWithMapOfArrays()
        {
            string idl = @"service testService
                            {
                                void testFunction1(map<string, array<i32>> param);
                            }";
            Assert.DoesNotThrow(() => idlParser.ParseIDL(idl));
            var testFunction = idlParser.CurrentlyParsedSinTD.SINFONIServices
                .GetService("testService").GetServiceFunction("testFunction1");
            var parameter = testFunction.Parameters["param"];
            Assert.AreEqual(typeof(SinTDMap), parameter.GetType());

            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("string"),
                ((SinTDMap)parameter).keyType);

            Assert.AreEqual(typeof(SinTDArray),
                ((SinTDMap)parameter).elementType.GetType());
        }

        [Test()]
        public void ShouldParseServiceFunctionWithArrayOfMaps()
        {
            string idl = @"service testService
                            {
                                void testFunction1(array<map<string, i32>> param);
                            }";
            Assert.DoesNotThrow(() => idlParser.ParseIDL(idl));
            var testFunction = idlParser.CurrentlyParsedSinTD.SINFONIServices
                .GetService("testService").GetServiceFunction("testFunction1");
            var parameter = testFunction.Parameters["param"];
            Assert.AreEqual(typeof(SinTDArray), parameter.GetType());
            Assert.AreEqual(typeof(SinTDMap), ((SinTDArray)parameter).ElementType.GetType());
        }

        [Test()]
        public void ShouldParseServiceFunctionWithMultipleParameters()
        {
            string idl = @"service serverSync
                            {
                                void testFunction(string param1, array<i32> param2);                                
                            }";
            
            Assert.DoesNotThrow(() => idlParser.ParseIDL(idl));
            var testFunction = idlParser.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").GetServiceFunction("testFunction");
            var param1 = testFunction.Parameters["param1"];
            var param2 = testFunction.Parameters["param2"];

            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("string"), param1);
            Assert.AreEqual(typeof(SinTDArray), param2.GetType());
            Assert.AreEqual(idlParser.CurrentlyParsedSinTD.GetSinTDType("i32"), ((SinTDArray)param2).ElementType);
        }

        [Test()]
        public void ShouldHandleLineComments()
        {
            string idl = @"service serverSync
                            {
                                // This is a line comment
                                void testFunction(string param1, array<i32> param2);
                            }";
            Assert.DoesNotThrow(() => idlParser.ParseIDL(idl));
        }

        [Test()]
        public void ShouldHandleInlineComments()
        {
            string idl = @"service serverSync
                            {
                                void testFunction1(i32 p1 /* This is an inline comment */, string p2);
                                void testFunction2(string param1, array<i32> param2); // This is another inline comment
                            }";
            Assert.DoesNotThrow(() => idlParser.ParseIDL(idl));
            Assert.IsTrue(idlParser.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").ContainsServiceFunction("testFunction1"));
            var testFunction = idlParser.CurrentlyParsedSinTD
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
            Assert.DoesNotThrow(() => idlParser.ParseIDL(idl));
            Assert.IsTrue(idlParser.CurrentlyParsedSinTD
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
            Assert.DoesNotThrow(() => idlParser.ParseIDL(idl));
            Assert.IsTrue(idlParser.CurrentlyParsedSinTD
                .SINFONIServices.GetService("serverSync").ContainsServiceFunction("testFunction1"));
        }

        [Test()]
        public void ShouldParseExampleIDLWithoutError()
        {
            Assert.DoesNotThrow(() => idlParser.ParseIDL(exampleIDL));
            var services = idlParser.CurrentlyParsedSinTD.SINFONIServices.services;
            var types = idlParser.CurrentlyParsedSinTD.registeredTypes;
        }
    }
}
