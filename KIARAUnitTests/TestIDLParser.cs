using System;
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
            Assert.Contains("intValue", KTD.Instance.GetKtdType("BaseStruct").members.Keys);
            Assert.AreEqual(KTD.Instance.GetKtdType("i32"), KTD.Instance.GetKtdType("BaseStruct").members["intValue"]);
        }

        [Test()]
        public void ShouldParseStructWithMap()
        {
            string idl = @"struct MapStruct {
                            map<string, i16> testMap;
                         }";

            IDLParser.Instance.parseIDL(idl);
            Assert.IsTrue(KTD.Instance.ContainsType("MapStruct"));
            Assert.Contains("testMap", KTD.Instance.GetKtdType("MapStruct").members.Keys);

            var parsedMap = KTD.Instance.GetKtdType("MapStruct").members["testMap"];
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
            Assert.Contains("testArray", KTD.Instance.GetKtdType("ArrayStruct").members.Keys);

            var parsedArray = KTD.Instance.GetKtdType("ArrayStruct").members["testArray"];
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
        public void ShouldParseExampleIDLWithoutError()
        {
            Assert.DoesNotThrow(() => IDLParser.Instance.parseIDL());
        }
    }
}
