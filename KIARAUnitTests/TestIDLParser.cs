using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA;
using NUnit.Framework;

namespace KIARAUnitTests
{
    [TestFixture()]
    class TestIDLParser
    {
        [Test()]
        public void ShouldParseCorrectArrayType()
        {
            var intArray = ArrayParser.Instance.ParseArray("array<i16>");
            Assert.AreEqual(KTD.Instance.GetType("i16"), intArray.elementType);
        }

        [Test()]
        public void ShouldParseCorrectMapKeyType()
        {
            var stringToIntMap = MapParser.Instance.ParseMap("map<string,i16>");
            Assert.AreEqual(KTD.Instance.GetType("string"), stringToIntMap.keyType);
        }

        [Test()]
        public void ShouldParseCorrectMapValueType()
        {
            var stringToIntMap = MapParser.Instance.ParseMap("map<string,i16>");
            Assert.AreEqual(KTD.Instance.GetType("i16"), stringToIntMap.elementType);
        }

        [Test()]
        public void ShouldParseArrayDefinitionWithSpaces()
        {
            var intArray = ArrayParser.Instance.ParseArray("array < i16 >");
            Assert.AreEqual(KTD.Instance.GetType("i16"), intArray.elementType);
        }

        [Test()]
        public void ShouldParseMapDefinitionsWithSpaces()
        {
            var stringToIntMap = MapParser.Instance.ParseMap("map < string, i16 >");
            Assert.AreEqual(KTD.Instance.GetType("string"), stringToIntMap.keyType);
            Assert.AreEqual(KTD.Instance.GetType("i16"), stringToIntMap.elementType);
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
            Assert.Contains("intValue", KTD.Instance.GetType("BaseStruct").members.Keys);
            Assert.AreEqual(KTD.Instance.GetType("i32"), KTD.Instance.GetType("BaseStruct").members["intValue"]);
        }

        [Test()]
        public void ShouldParseStructWithMap()
        {
            string idl = @"struct MapStruct {
                            map<string, i16> testMap;
                         }";

            IDLParser.Instance.parseIDL(idl);
            Assert.IsTrue(KTD.Instance.ContainsType("MapStruct"));
            Assert.Contains("testMap", KTD.Instance.GetType("MapStruct").members.Keys);

            var parsedMap = KTD.Instance.GetType("MapStruct").members["testMap"];
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
            Assert.Contains("testArray", KTD.Instance.GetType("ArrayStruct").members.Keys);

            var parsedArray = KTD.Instance.GetType("ArrayStruct").members["testArray"];
            Assert.AreEqual(typeof(KtdArray), parsedArray.GetType());
        }
    }
}
