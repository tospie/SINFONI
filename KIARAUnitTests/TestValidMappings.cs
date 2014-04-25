using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA;
using NUnit;
using NUnit.Framework;

namespace KIARAUnitTests
{
    [TestFixture()]
    class TestValidMappings
    {

        KtdType i16;
        KtdType i32;
        KtdType i64;

        KtdType ui16;
        KtdType ui32;
        KtdType ui64;

        KtdType ktd_double;
        KtdType ktd_float;

        KtdType ktd_string;
        KtdType ktd_bool;

        KtdType intStruct;
        KtdType nestedStruct;

        KtdArray ktd_array;
        KtdMap map;

        KtdArray ktd_arrayOfStructs;
        KtdMap ktd_mapWithStructElements;
        KtdMap ktd_mapWithStructKeys;

        struct testStruct
        {
            public int x;
            public int y;
            public bool affe;
        }

        struct nestedTestStruct
        {
            public string name;
            public bool b;
            public testStruct s;
        }

        class testClass
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        class nestedTestClass
        {
            public string name { get; set; }
            public bool b;

            public testClass s;
        }

        [SetUp()]
        public void InitTests()
        {
            i16 = new KtdType();
            i16.Name = "i16";

            i32 = new KtdType();
            i32.Name = "i32";

            i64 = new KtdType();
            i64.Name = "i64";

            ui16 = new KtdType("uint16");
            ui32 = new KtdType("uint32");
            ui64 = new KtdType("uint64");

            ktd_double = new KtdType("double");

            ktd_float = new KtdType("float");

            ktd_string = new KtdType();
            ktd_string.Name = "string";

            ktd_bool = new KtdType();
            ktd_bool.Name = "boolean";

            intStruct = new KtdType();
            intStruct.members["x"] = i32;
            intStruct.members["y"] = i32;
            intStruct.Name = "intStruct";

            nestedStruct = new KtdType();
            nestedStruct.members["name"] = ktd_string;
            nestedStruct.members["b"] = ktd_bool;
            nestedStruct.members["s"] = intStruct;

            ktd_array = new KtdArray();
            ktd_array.Name = "array<int32>";
            ktd_array.elementType = i32;

            map = new KtdMap();
            map.keyType = ktd_string;
            map.elementType = intStruct;

            ktd_arrayOfStructs = new KtdArray();
            ktd_arrayOfStructs.Name = "array<nestedStruct>";
            ktd_arrayOfStructs.elementType = nestedStruct;

            ktd_mapWithStructElements = new KtdMap();
            ktd_mapWithStructElements.elementType = nestedStruct;
            ktd_mapWithStructElements.keyType = ktd_string;

            ktd_mapWithStructKeys = new KtdMap();
            ktd_mapWithStructKeys.elementType = ktd_string;
            ktd_mapWithStructKeys.keyType = nestedStruct;
        }

        [Test()]
        public void ShouldAssignToInt16()
        {            
            Assert.IsTrue(i16.canBeAssignedFromType(typeof(short)));
        }
        
        [Test()]
        public void ShouldAssignToInt32()
        {                    
            Assert.IsTrue(i32.canBeAssignedFromType(typeof(int)));
        }

        [Test()]
        public void ShouldAssignToInt64()
        {
            Assert.IsTrue(i64.canBeAssignedFromType(typeof(long)));
        }


        [Test()]
        public void ShouldAssignToUInt16()
        {
            Assert.IsTrue(ui16.canBeAssignedFromType(typeof(ushort)));
        }

        [Test()]
        public void ShouldAssignToUInt32()
        {
            Assert.IsTrue(ui32.canBeAssignedFromType(typeof(uint)));
        }

        [Test()]
        public void ShouldAssignToUInt64()
        {
            Assert.IsTrue(ui64.canBeAssignedFromType(typeof(ulong)));
        }

        [Test()]
        public void ShouldAssignToFloat()
        {
            Assert.IsTrue(ktd_float.canBeAssignedFromType(typeof(float)));
        }

        [Test()]
        public void ShouldAssignToDouble()
        {
            Assert.IsTrue(ktd_double.canBeAssignedFromType(typeof(double)));
        }

        [Test()]
        public void CanMapStruct()
        {            
            Assert.IsTrue(intStruct.canBeAssignedFromType(typeof(testStruct)));
        }

        [Test()]
        public void CanMapStructsOfStructs()
        {         
            Assert.IsTrue(nestedStruct.canBeAssignedFromType(typeof(nestedTestStruct)));
        }
           
        [Test()]
        public void CanMapClassWithProperties()
        {
            var c = new testClass { y = 2 };
            Assert.IsTrue(intStruct.canBeAssignedFromType(c.GetType()));
        }

        [Test()]
        public void CanAssignNestedClass()
        {
            Assert.IsTrue(nestedStruct.canBeAssignedFromType(typeof(nestedTestClass)));
        }

        [Test()]
        public void ShouldMatchArrayTypes()
        {
            Assert.IsTrue(ktd_array.canBeAssignedFromType(typeof(int[])));
            Assert.IsTrue(ktd_array.canBeAssignedFromType(typeof(List<int>)));
            Assert.IsTrue(ktd_array.canBeAssignedFromType(typeof(ISet<int>)));            
        }

        [Test()]
        public void ShouldMatchDictionaryTypes()
        {         
            Assert.IsTrue(map.canBeAssignedFromType(typeof(Dictionary<string, testClass>)));
        }

        [Test()]
        public void ShouldReturnFalseForWrongArrayType()
        {
            Assert.IsFalse(ktd_array.canBeAssignedFromType(typeof(float[])));
            Assert.IsFalse(ktd_array.canBeAssignedFromType(typeof(List<float>)));
            Assert.IsFalse(ktd_array.canBeAssignedFromType(typeof(ISet<float>)));      
        }

        [Test()]
        public void ShouldReturnFalseForWrongDictionaryKey()
        {
            Assert.IsFalse(map.canBeAssignedFromType(typeof(Dictionary<int, testClass>)));
        }

        [Test()]
        public void ShouldReturnFalseForWrongDictionaryValue()
        {
            Assert.IsFalse(map.canBeAssignedFromType(typeof(Dictionary<string, nestedTestClass>)));
        }

        [Test()]
        public void ShouldMapArrayElementsAsClassAndStruct()
        {
            Assert.IsTrue(ktd_arrayOfStructs.canBeAssignedFromType(typeof(List<nestedTestClass>)));
            Assert.IsTrue(ktd_arrayOfStructs.canBeAssignedFromType(typeof(List<nestedTestStruct>)));
        }

        [Test()]
        public void ShouldMapDictionaryElementsAsClassAndStruct()
        {
            Assert.IsTrue(ktd_mapWithStructElements.canBeAssignedFromType(typeof(Dictionary<string, nestedTestClass>)));
            Assert.IsTrue(ktd_mapWithStructElements.canBeAssignedFromType(typeof(Dictionary<string, nestedTestStruct>)));
        }

        [Test()]
        public void ShouldMapDictionaryKeysAsClassAndStruct()
        {
            Assert.IsTrue(ktd_mapWithStructKeys.canBeAssignedFromType(typeof(Dictionary<nestedTestClass, string>)));
            Assert.IsTrue(ktd_mapWithStructKeys.canBeAssignedFromType(typeof(Dictionary<nestedTestStruct, string>)));
        }

        [Test()]
        public void ShouldCacheValidMapping()
        {
            var KtdType = new KtdType("testType");
            KtdType.members.Add("i16", KTD.Instance.GetKtdType("i16"));
            KtdType.canBeAssignedFromType(typeof(List<nestedTestStruct>));
            Assert.Contains(typeof(List<nestedTestStruct>), KtdType.validMappings.Keys);
            Assert.IsFalse(KtdType.validMappings[typeof(List<nestedTestStruct>)]);
        }

        [Test()]
        public void ServiceMatchParametersToEmptyService()
        {
            ServiceFunctionDescription noParameterSF =
                new ServiceFunctionDescription("testfunction", new KtdType("void"));

            Assert.IsTrue(noParameterSF.CanBeCalledWithParameters(new object[] {}));
        }

        [Test()]
        public void ServiceShouldNotMatchCallWithWrongParameterCount()
        {
            ServiceFunctionDescription noParameterSF =
                new ServiceFunctionDescription("testfunction", new KtdType("void"));
            ServiceFunctionDescription oneParameterSF =
                new ServiceFunctionDescription("testfunction2", new KtdType("void"));
            oneParameterSF.parameters.Add("parameter", i32);

            Assert.IsFalse(noParameterSF.CanBeCalledWithParameters(new object[] {1}));
            Assert.IsFalse(oneParameterSF.CanBeCalledWithParameters(new object[] { }));
        }

        [Test()]
        public void ServiceShouldMatchCorrectTypedParameters()
        {
            ServiceFunctionDescription oneParameterSF =
                new ServiceFunctionDescription("testfunction1", new KtdType("void"));
            oneParameterSF.parameters.Add("parameter", i32);
            Assert.IsTrue(oneParameterSF.CanBeCalledWithParameters(new object[] { 1 }));
        }

        [Test()]
        public void ServiceShouldReturnFalseForMatchingParametersWithWrongTypes()
        {
            ServiceFunctionDescription oneParameterSF =
                new ServiceFunctionDescription("testfunction1", new KtdType("void"));
            oneParameterSF.parameters.Add("parameter", i32);
            Assert.IsFalse(oneParameterSF.CanBeCalledWithParameters(new object[] { "Hello World" }));
        }

        [Test()]
        public void ServiceShouldReturnTrueForMatchingMultipleCorrectParameters()
        {
            ServiceFunctionDescription oneParameterSF =
                new ServiceFunctionDescription("testfunction1", new KtdType("void"));
            oneParameterSF.parameters.Add("intParameter", i32);
            oneParameterSF.parameters.Add("stringParameter", ktd_string);
            oneParameterSF.parameters.Add("floatParameter", ktd_float);
            Assert.IsTrue(oneParameterSF.CanBeCalledWithParameters(new object[] { 1, "Hello World", 1.0f }));
        }

        [Test()]
        public void ServiceShouldReturnTrueForMatchWithCorrectReturntype()
        {
            ServiceFunctionDescription sf = new ServiceFunctionDescription("testfunction", i32);
            Assert.True(sf.CanBeCalledWithReturnType(typeof(int)));
        }

        [Test()]
        public void ServiceShouldReturnFalseForMatchWithWrongReturntype()
        {
            ServiceFunctionDescription sf = new ServiceFunctionDescription("testfunction", ktd_float);
            Assert.False(sf.CanBeCalledWithReturnType(typeof(int)));
        }
    }
}
