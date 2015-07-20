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
using NUnit;
using NUnit.Framework;

namespace SINFONIUnitTests
{
    [TestFixture()]
    class TestValidMappings
    {

        SinTDType i16;
        SinTDType i32;
        SinTDType i64;

        SinTDType ui16;
        SinTDType ui32;
        SinTDType ui64;

        SinTDType SinTD_double;
        SinTDType SinTD_float;

        SinTDType SinTD_string;
        SinTDType SinTD_bool;

        SinTDStruct intStruct;
        SinTDStruct nestedStruct;
        SinTDStruct Vector;

        SinTDArray SinTD_array;
        SinTDMap map;

        SinTDArray SinTD_arrayOfStructs;
        SinTDMap SinTD_mapWithStructElements;
        SinTDMap SinTD_mapWithStructKeys;

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
            i16 = new SinTDType("i16");

            i32 = new SinTDType("i32");

            i64 = new SinTDType("i64");

            ui16 = new SinTDType("u16");
            ui32 = new SinTDType("u32");
            ui64 = new SinTDType("u64");

            SinTD_double = new SinTDType("double");

            SinTD_float = new SinTDType("float");

            SinTD_string = new SinTDType();
            SinTD_string.Name = "string";

            SinTD_bool = new SinTDType();
            SinTD_bool.Name = "boolean";

            intStruct = new SinTDStruct("intStruct");
            intStruct.members["x"] = i32;
            intStruct.members["y"] = i32;

            nestedStruct = new SinTDStruct("nestedStruct");
            nestedStruct.members["name"] = SinTD_string;
            nestedStruct.members["b"] = SinTD_bool;
            nestedStruct.members["s"] = intStruct;

            SinTD_array = new SinTDArray();
            SinTD_array.Name = "array<int32>";
            SinTD_array.elementType = i32;

            map = new SinTDMap();
            map.keyType = SinTD_string;
            map.elementType = intStruct;

            SinTD_arrayOfStructs = new SinTDArray();
            SinTD_arrayOfStructs.Name = "array<nestedStruct>";
            SinTD_arrayOfStructs.elementType = nestedStruct;

            SinTD_mapWithStructElements = new SinTDMap();
            SinTD_mapWithStructElements.elementType = nestedStruct;
            SinTD_mapWithStructElements.keyType = SinTD_string;

            SinTD_mapWithStructKeys = new SinTDMap();
            SinTD_mapWithStructKeys.elementType = SinTD_string;
            SinTD_mapWithStructKeys.keyType = nestedStruct;

            Vector = new SinTDStruct("Vector");
            Vector.members.Add("x", SinTD_float);
            Vector.members.Add("y", SinTD_float);
            Vector.members.Add("z", SinTD_float);
        }

        [Test()]
        public void ShouldAssignToInt16()
        {
            Assert.IsTrue(i16.CanBeAssignedFromType(typeof(short)));
        }
        
        [Test()]
        public void ShouldAssignToInt32()
        {                    
            Assert.IsTrue(i32.CanBeAssignedFromType(typeof(int)));
        }

        [Test()]
        public void ShouldAssignToInt64()
        {
            Assert.IsTrue(i64.CanBeAssignedFromType(typeof(long)));
        }


        [Test()]
        public void ShouldAssignToUInt16()
        {
            Assert.IsTrue(ui16.CanBeAssignedFromType(typeof(ushort)));
        }

        [Test()]
        public void ShouldAssignToUInt32()
        {
            Assert.IsTrue(ui32.CanBeAssignedFromType(typeof(uint)));
        }

        [Test()]
        public void ShouldAssignToUInt64()
        {
            Assert.IsTrue(ui64.CanBeAssignedFromType(typeof(ulong)));
        }

        [Test()]
        public void ShouldAssignToFloat()
        {
            Assert.IsTrue(SinTD_float.CanBeAssignedFromType(typeof(float)));
        }

        [Test()]
        public void ShouldAssignToDouble()
        {
            Assert.IsTrue(SinTD_double.CanBeAssignedFromType(typeof(double)));
        }

        [Test()]
        public void CanMapStruct()
        {
            Assert.IsTrue(intStruct.CanBeAssignedFromType(typeof(testStruct)));
        }

        [Test()]
        public void CanMapStructsOfStructs()
        {
            Assert.IsTrue(nestedStruct.CanBeAssignedFromType(typeof(nestedTestStruct)));
        }
           
        [Test()]
        public void CanMapClassWithProperties()
        {
            var c = new testClass { y = 2 };
            Assert.IsTrue(intStruct.CanBeAssignedFromType(c.GetType()));
        }

        [Test()]
        public void CanAssignNestedClass()
        {
            Assert.IsTrue(nestedStruct.CanBeAssignedFromType(typeof(nestedTestClass)));
        }

        [Test()]
        public void ShouldMatchArrayTypes()
        {
            Assert.IsTrue(SinTD_array.CanBeAssignedFromType(typeof(int[])));
            Assert.IsTrue(SinTD_array.CanBeAssignedFromType(typeof(List<int>)));
            Assert.IsTrue(SinTD_array.CanBeAssignedFromType(typeof(ISet<int>)));
        }

        [Test()]
        public void ShouldMatchDictionaryTypes()
        {         
            Assert.IsTrue(map.CanBeAssignedFromType(typeof(Dictionary<string, testClass>)));
        }

        [Test()]
        public void ShouldReturnFalseForWrongArrayType()
        {
            Assert.IsFalse(SinTD_array.CanBeAssignedFromType(typeof(float[])));
            Assert.IsFalse(SinTD_array.CanBeAssignedFromType(typeof(List<float>)));
            Assert.IsFalse(SinTD_array.CanBeAssignedFromType(typeof(ISet<float>)));
        }

        [Test()]
        public void ShouldReturnFalseForWrongDictionaryKey()
        {
            Assert.IsFalse(map.CanBeAssignedFromType(typeof(Dictionary<int, testClass>)));
        }

        [Test()]
        public void ShouldReturnFalseForWrongDictionaryValue()
        {
            Assert.IsFalse(map.CanBeAssignedFromType(typeof(Dictionary<string, nestedTestClass>)));
        }

        [Test()]
        public void ShouldMapArrayElementsAsClassAndStruct()
        {
            Assert.IsTrue(SinTD_arrayOfStructs.CanBeAssignedFromType(typeof(List<nestedTestClass>)));
            Assert.IsTrue(SinTD_arrayOfStructs.CanBeAssignedFromType(typeof(List<nestedTestStruct>)));
        }

        [Test()]
        public void ShouldMapDictionaryElementsAsClassAndStruct()
        {
            Assert.IsTrue(SinTD_mapWithStructElements.CanBeAssignedFromType(typeof(Dictionary<string, nestedTestClass>)));
            Assert.IsTrue(SinTD_mapWithStructElements.CanBeAssignedFromType(typeof(Dictionary<string, nestedTestStruct>)));
        }

        [Test()]
        public void ShouldMapDictionaryKeysAsClassAndStruct()
        {
            Assert.IsTrue(SinTD_mapWithStructKeys.CanBeAssignedFromType(typeof(Dictionary<nestedTestClass, string>)));
            Assert.IsTrue(SinTD_mapWithStructKeys.CanBeAssignedFromType(typeof(Dictionary<nestedTestStruct, string>)));
        }

        [Test()]
        public void ShouldMapDictionaryToStruct()
        {
            Assert.IsTrue(Vector.CanBeAssignedFromType(typeof(Dictionary<string, float>)));
        }

        [Test()]
        public void ShouldCacheValidMapping()
        {
            var SinTDType = new SinTDStruct("testType");
            SinTDType.members.Add("i16", i16);
            SinTDType.CanBeAssignedFromType(typeof(List<nestedTestStruct>));
            Assert.Contains(typeof(List<nestedTestStruct>), SinTDType.validMappings.Keys);
            Assert.IsFalse(SinTDType.validMappings[typeof(List<nestedTestStruct>)]);
        }

        [Test()]
        public void ServiceMatchParametersToEmptyService()
        {
            ServiceFunctionDescription noParameterSF =
                new ServiceFunctionDescription("testfunction", new SinTDType("void"));

            Assert.IsTrue(noParameterSF.CanBeCalledWithParameters(new object[] {}));
        }

        [Test()]
        public void ServiceShouldNotMatchCallWithWrongParameterCount()
        {
            ServiceFunctionDescription noParameterSF =
                new ServiceFunctionDescription("testfunction", new SinTDType("void"));
            ServiceFunctionDescription oneParameterSF =
                new ServiceFunctionDescription("testfunction2", new SinTDType("void"));
            oneParameterSF.Parameters.Add("parameter", i32);

            Assert.IsFalse(noParameterSF.CanBeCalledWithParameters(new object[] {1}));
            Assert.IsFalse(oneParameterSF.CanBeCalledWithParameters(new object[] { }));
        }

        [Test()]
        public void ServiceShouldMatchCorrectTypedParameters()
        {
            ServiceFunctionDescription oneParameterSF =
                new ServiceFunctionDescription("testfunction1", new SinTDType("void"));
            oneParameterSF.Parameters.Add("parameter", i32);
            Assert.IsTrue(oneParameterSF.CanBeCalledWithParameters(new object[] { 1 }));
        }

        [Test()]
        public void ServiceShouldReturnFalseForMatchingParametersWithWrongTypes()
        {
            ServiceFunctionDescription oneParameterSF =
                new ServiceFunctionDescription("testfunction1", new SinTDType("void"));
            oneParameterSF.Parameters.Add("parameter", i32);
            Assert.IsFalse(oneParameterSF.CanBeCalledWithParameters(new object[] { "Hello World" }));
        }

        [Test()]
        public void ServiceShouldReturnTrueForMatchingMultipleCorrectParameters()
        {
            ServiceFunctionDescription oneParameterSF =
                new ServiceFunctionDescription("testfunction1", new SinTDType("void"));
            oneParameterSF.Parameters.Add("intParameter", i32);
            oneParameterSF.Parameters.Add("stringParameter", SinTD_string);
            oneParameterSF.Parameters.Add("floatParameter", SinTD_float);
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
            ServiceFunctionDescription sf = new ServiceFunctionDescription("testfunction", SinTD_float);
            Assert.False(sf.CanBeCalledWithReturnType(typeof(int)));
        }
    }
}
