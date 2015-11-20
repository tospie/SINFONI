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
    public class TestTypeInstances
    {
        SinTDStruct intStruct;
        SinTDStruct aStruct;
        SinTDStruct mStruct;
        SinTDStruct sStruct;

        SinTDType i32;
        SinTDType SinTDString;
        SinTDType SinTDBool;

        struct testStruct
        {
            public int x;
            public int y;
        }

        struct arrayStruct
        {
            public int[] arr;
        }

        struct mapStruct
        {
            public Dictionary<string, bool> map;
        }

        struct structStruct
        {
            public testStruct child;
        }

        class testClassProps
        {
            public int x { get; set; }
            public int y { get; set; }
        }

        class testClassFields
        {
            public int x;
            public int y;
        }

        class testClassBoth
        {
            public int x;
            public int y { get; set; }
        }

        SinTD SinTDInstance = new SinTD();

        [TestFixtureSetUp()]
        public void TestSetUp()
        {
            SinTDInstance = new SinTD();

            i32 = SinTDInstance.GetSinTDType("i32");
            SinTDString = SinTDInstance.GetSinTDType("string");
            SinTDBool = SinTDInstance.GetSinTDType("boolean");

            intStruct = new SinTDStruct("instancesIntStruct");
            intStruct.AddMember("x", i32);
            intStruct.AddMember("y", i32);

            aStruct = new SinTDStruct("instancesArrayStruct");
            aStruct.AddMember("arr", new SinTDArray(i32));
            SinTDInstance.RegisterType(aStruct);

            mStruct = new SinTDStruct("isntancesMapStruct");
            mStruct.AddMember("map", new SinTDMap(SinTDString, SinTDBool));
            SinTDInstance.RegisterType(mStruct);

            sStruct = new SinTDStruct("instancesStructStruct");
            sStruct.AddMember("child", intStruct);
            SinTDInstance.RegisterType(sStruct);
        }

        [Test()]
        public void ShouldAssignValueToInt16()
        {
            SinTDType i16 = SinTDInstance.GetSinTDType("i16");
            short shortInstance = (short)i16.AssignValuesFromObject((short)1);
            Assert.AreEqual((short)1, shortInstance);
        }

        [Test()]
        public void ShouldAssingI16toNativeShort()
        {
            SinTDType i16 = SinTDInstance.GetSinTDType("i16");
            Assert.AreEqual((short)1, i16.AssignValuesToNativeType(1, typeof(short)));
        }

        [Test()]
        public void ShouldAssignValueToUInt16()
        {
            SinTDType u16 = SinTDInstance.GetSinTDType("u16");
            ushort ushortInstance = (ushort)u16.AssignValuesFromObject((ushort)1);
            Assert.AreEqual(1, ushortInstance);
        }

        [Test()]
        public void ShouldAssignUInt16ValueToNativeUshort()
        {
            SinTDType u16 = SinTDInstance.GetSinTDType("u16");
            Assert.AreEqual(1, u16.AssignValuesToNativeType(1, typeof(ushort)));
        }

        [Test()]
        public void ShouldAssignValueToInt32()
        {
            SinTDType i32 = SinTDInstance.GetSinTDType("i32");
            int intInstance = (int)i32.AssignValuesFromObject(1);
            Assert.AreEqual(1, intInstance);
        }

        [Test()]
        public void ShouldAssignInt32ToNativeInt()
        {
            SinTDType i32 = SinTDInstance.GetSinTDType("i32");
            Assert.AreEqual(1, i32.AssignValuesToNativeType(1, typeof(int)));
        }

        [Test()]
        public void ShouldAssignValueToUInt32()
        {
            SinTDType u32 = SinTDInstance.GetSinTDType("u32");
            uint uintInstance = (uint)u32.AssignValuesFromObject((uint)1);
            Assert.AreEqual(1, uintInstance);
        }

        [Test()]
        public void ShouldAssignUInt32ToNativeUInt()
        {
            SinTDType u32 = SinTDInstance.GetSinTDType("u32");
            Assert.AreEqual(1, u32.AssignValuesToNativeType(1, typeof(uint)));
        }

        [Test()]
        public void ShouldAssignValueToInt64()
        {
            SinTDType i64 = SinTDInstance.GetSinTDType("i64");
            long int64Instance = (long)i64.AssignValuesFromObject((long)1);
            Assert.AreEqual((long)1, int64Instance);
        }

        [Test()]
        public void ShouldAssignInt64ToNativeLong()
        {
            SinTDType i64 = SinTDInstance.GetSinTDType("i64");
            Assert.AreEqual((long)1, i64.AssignValuesToNativeType(1, typeof(long)));
        }

        [Test()]
        public void ShouldAssignValueToUInt64()
        {
            SinTDType u64 = SinTDInstance.GetSinTDType("u64");
            ulong u64Instance = (ulong)u64.AssignValuesFromObject((ulong)1);
            Assert.AreEqual((ulong)1, u64Instance);
        }

        [Test()]
        public void ShouldAssignU64ToNativeUlong()
        {
            SinTDType u64 = SinTDInstance.GetSinTDType("u64");
            Assert.AreEqual((ulong)1, u64.AssignValuesToNativeType(1, typeof(ulong)));
        }

        [Test()]
        public void ShouldAssignValueToFloat()
        {
            SinTDType f = SinTDInstance.GetSinTDType("float");
            float fInstance = (float)f.AssignValuesFromObject(1.0f);
            Assert.AreEqual(1.0f, fInstance);
        }

        [Test()]
        public void ShouldAssignFloatToNativeFloat()
        {
            SinTDType f = SinTDInstance.GetSinTDType("float");
           Assert.AreEqual(1.0f, f.AssignValuesToNativeType(1.0, typeof(float)));
        }

        [Test()]
        public void ShouldAssignValueToDouble()
        {
            SinTDType d = SinTDInstance.GetSinTDType("double");
            double dInstance = (double)d.AssignValuesFromObject(1.0);
            Assert.AreEqual(1.0, dInstance);
        }

        [Test()]
        public void ShouldAssignDoubleToNativeDouble()
        {
            SinTDType d = SinTDInstance.GetSinTDType("double");
            Assert.AreEqual(1.0, d.AssignValuesToNativeType(1.0, typeof(double)));
        }

        [Test()]
        public void ShouldAssignBooleanToNativeBool()
        {
            SinTDType b = SinTDInstance.GetSinTDType("boolean");
            Assert.AreEqual(true, b.AssignValuesToNativeType(true, typeof(bool)));
        }

        [Test()]
        public void ShouldAssignStringToNativeString()
        {
            SinTDType s = SinTDInstance.GetSinTDType("string");
            Assert.AreEqual("Hello World", s.AssignValuesToNativeType("Hello World", typeof(string)));
        }

        [Test()]
        public void ShouldAssignValueToAny()
        {
            SinTDType any = SinTDInstance.GetSinTDType("any");
            var anyValue = 1;

            var stringInstance = any.AssignValuesFromObject(anyValue);
            Assert.AreEqual(anyValue, stringInstance);
        }

        [Test()]
        public void ShouldAssignAnyToNative()
        {
            SinTDType any = SinTDInstance.GetSinTDType("any");
            var anyValue = 1;

            Assert.AreEqual(anyValue, any.AssignValuesToNativeType(anyValue, anyValue.GetType()));
        }

        [Test()]
        public void ShouldAssignArrayToArray()
        {
            var intArray = new int[] { 1 };
            SinTDArray array = new SinTDArray();
            array.ElementType = SinTDInstance.GetSinTDType("i32");

            var arrayInstance =  array.AssignValuesFromObject(intArray) as object[];
            Assert.AreEqual(1, arrayInstance.Length);
            Assert.AreEqual(1, (int)arrayInstance[0]);
        }

        [Test()]
        public void ShouldAssignArrayToNativeArray()
        {
            var intArray = new int[0];
            SinTDArray array = new SinTDArray();
            array.ElementType = SinTDInstance.GetSinTDType("i32");
            var test = array.AssignValuesToNativeType(new int[1]{1}, intArray.GetType());
            Assert.AreEqual(1, ((int[])test).Length);
            Assert.AreEqual(1, ((int[])test)[0]);
        }

        [Test()]
        public void ShouldAssignListToArray()
        {
            var intList = new List<int>{ 1 };
            SinTDArray array = new SinTDArray();
            array.ElementType = SinTDInstance.GetSinTDType("i32");

            var arrayInstance = array.AssignValuesFromObject(intList) as object[];
            Assert.AreEqual(1, arrayInstance.Length);
            Assert.AreEqual(1, (int)arrayInstance[0]);
        }

        [Test()]
        public void ShouldAssignArrayToNativeList()
        {
            var intList = new List<int>();
            SinTDArray array = new SinTDArray();
            array.ElementType = SinTDInstance.GetSinTDType("i32");
            List<int> test = (List<int>)array.AssignValuesToNativeType(new int[1] { 1 }, intList.GetType());
            Assert.AreEqual(1, test.Count);
            Assert.AreEqual(1, test[0]);
        }

        [Test()]
        public void ShouldAssignSetToArray()
        {
            var intSet = new HashSet<int> { 1 };
            SinTDArray array = new SinTDArray();
            array.ElementType = SinTDInstance.GetSinTDType("i32");

            object[] arrayInstance = array.AssignValuesFromObject(intSet) as object[];
            Assert.AreEqual(1, arrayInstance.Length);
            Assert.AreEqual(1, (int)arrayInstance[0]);
        }

        [Test()]
        public void ShouldAssignArrayToNativeSet()
        {
            var intSet = new HashSet<int>();
            SinTDArray array = new SinTDArray();
            array.ElementType = SinTDInstance.GetSinTDType("i32");
            ISet<int> test = (HashSet<int>)array.AssignValuesToNativeType(new int[1] { 1 }, intSet.GetType());
            Assert.AreEqual(1, test.Count);
            Assert.AreEqual(1, test.ElementAt(0));
        }

        [Test()]
        public void ShouldAssignArrayOfArraysToNative()
        {
            var intArray = new int[1][];
            intArray[0] = new int[1] { 2 };
            SinTDArray innerArray = new SinTDArray();
            innerArray.ElementType = SinTDInstance.GetSinTDType("i32");
            SinTDArray outerArray = new SinTDArray();
            outerArray.ElementType = innerArray;
            int[][] test = (int[][])outerArray.AssignValuesToNativeType(intArray, intArray.GetType());
            Assert.AreEqual(1, test.Length);
            Assert.AreEqual(1, test[0].Length);
            Assert.AreEqual(new int[1] { 2 }, test[0]);
        }

        [Test()]
        public void ShouldAssignArrayOfListsToNative()
        {
            var intArray = new int[1][];
            intArray[0] = new int[1] { 2 };
            List<int>[] listArray = new List<int> [1] { new List<int> { 1 } };
            SinTDArray innerArray = new SinTDArray();
            innerArray.ElementType = SinTDInstance.GetSinTDType("i32");
            SinTDArray outerArray = new SinTDArray();
            outerArray.ElementType = innerArray;
            List<int>[] test = (List<int>[])outerArray.AssignValuesToNativeType(intArray, listArray.GetType());
            Assert.IsAssignableFrom<List<int>>(test[0]);
            Assert.AreEqual(2, test[0][0]);
        }

        [Test()]
        public void ShouldAssignListOfArraysToNative()
        {
            var intArray = new int[1][];
            intArray[0] = new int[1] { 2 };
            List<int[]> arrayList = new List<int[]>{ new int[1] { 1 } };
            SinTDArray innerArray = new SinTDArray();
            innerArray.ElementType = SinTDInstance.GetSinTDType("i32");
            SinTDArray outerArray = new SinTDArray();
            outerArray.ElementType = innerArray;
            List<int[]> test = (List<int[]>)outerArray.AssignValuesToNativeType(intArray, arrayList.GetType());
            Assert.IsAssignableFrom<int[]>(test[0]);
            Assert.AreEqual(2, test[0][0]);
        }

        [Test()]
        public void ShouldAssignArrayOfSetsToNative()
        {
            var intArray = new int[1][];
            intArray[0] = new int[1] { 2 };
            ISet<int>[] setArray = new HashSet<int>[1] { new HashSet<int> { 1 } };
            SinTDArray innerArray = new SinTDArray();
            innerArray.ElementType = SinTDInstance.GetSinTDType("i32");
            SinTDArray outerArray = new SinTDArray();
            outerArray.ElementType = innerArray;
            HashSet<int>[] test = (HashSet<int>[])outerArray.AssignValuesToNativeType(intArray, setArray.GetType());
            Assert.IsAssignableFrom<HashSet<int>>(test[0]);
            Assert.AreEqual(2, test[0].ElementAt(0));
        }

        [Test()]
        public void ShouldAssignLSetOfArraysToNative()
        {
            var intArray = new int[1][];
            intArray[0] = new int[1] { 2 };
            ISet<int[]> arraySet = new HashSet<int[]> { new int[1] { 1 } };
            SinTDArray innerArray = new SinTDArray();
            innerArray.ElementType = SinTDInstance.GetSinTDType("i32");
            SinTDArray outerArray = new SinTDArray();
            outerArray.ElementType = innerArray;
            ISet<int[]> test = (HashSet<int[]>)outerArray.AssignValuesToNativeType(intArray, arraySet.GetType());
            Assert.IsAssignableFrom<int[]>(test.ElementAt(0));
            Assert.AreEqual(2, test.ElementAt(0)[0]);
        }

        [Test()]
        public void ShouldAssignEmptyArray()
        {
            var intArray = new int[] {};
            SinTDArray array = new SinTDArray();
            array.ElementType = SinTDInstance.GetSinTDType("i32");

            object[] arrayInstance = array.AssignValuesFromObject(intArray) as object[];
            Assert.AreEqual(0, arrayInstance.Length);
        }

        [Test()]
        public void ShouldThrowExceptionWhenAssigningNonEnumerableToArray()
        {
            SinTDArray array = new SinTDArray();
            array.ElementType = SinTDInstance.GetSinTDType("i32");

            Assert.Throws<TypeCastException>(
                () => { object[] arrayInstance = array.AssignValuesFromObject(1) as object[]; });
        }

        [Test()]
        public void ShouldThrowExceptionWhenAssigningArrayWithWrongElementType()
        {
            var floatArray = new float[] { 1f };
            SinTDArray array = new SinTDArray();
            array.ElementType = SinTDInstance.GetSinTDType("i32");

            Assert.Throws<TypeCastException>(
                () => { var arrayInstance = array.AssignValuesFromObject(floatArray) as object[]; });
        }

        [Test()]
        public void ShouldAssignDictionaryOfBaseTypesToMap()
        {
            var baseTypeDictionary = new Dictionary<string, int> {
                {"test", 1}
            };

            SinTDMap SinTDMap = new SinTDMap(SinTDInstance.GetSinTDType("string"), SinTDInstance.GetSinTDType("i32"));

            Dictionary<object, object> mapInstance
                = SinTDMap.AssignValuesFromObject(baseTypeDictionary) as Dictionary<object, object>;

            Assert.AreEqual(1, mapInstance.Values.Count);
            Assert.AreEqual("test", (mapInstance.Keys.ElementAt(0)));
            Assert.AreEqual(1, (mapInstance.Values.ElementAt(0)));
        }

        [Test()]
        public void ShouldAssignMapOfBaseTypesToDictionary()
        {
            var baseTypeDictionary = new Dictionary<string, int> {
                {"test", 1}
            };

            SinTDMap SinTDMap = new SinTDMap(SinTDInstance.GetSinTDType("string"), SinTDInstance.GetSinTDType("i32"));
            Dictionary<string, int> nativeDictionary =
                (Dictionary<string, int>)SinTDMap.AssignValuesToNativeType(baseTypeDictionary,
                typeof(Dictionary<string, int>));
            Assert.AreEqual(baseTypeDictionary, nativeDictionary);
        }

        [Test()]
        public void ShouldAssignedArrayTypedKeysToMap()
        {
            var arrayKeyDictionary = new Dictionary<int[], string>{
                {new int[]{1}, "test"}
            };

            SinTDArray keyArray = new SinTDArray();
            keyArray.ElementType = SinTDInstance.GetSinTDType("i32");
            SinTDMap SinTDMap = new SinTDMap(keyArray,  SinTDInstance.GetSinTDType("string"));

            Dictionary<object, object> mapInstance
                = SinTDMap.AssignValuesFromObject(arrayKeyDictionary) as Dictionary<object, object>;

            Assert.AreEqual(typeof(object[]), mapInstance.Keys.ElementAt(0).GetType());
            Assert.AreEqual(1,  ((object[])mapInstance.Keys.ElementAt(0)).Length);
            Assert.AreEqual(1, ((object[])mapInstance.Keys.ElementAt(0))[0]);
        }

        [Test()]
        public void ShouldAssingMapWithArrayKeysToNativeArrayKeyDictionary()
        {
            var arrayKeyDictionary = new Dictionary<int[], string>{
                {new int[]{1}, "test"}
            };

            SinTDArray keyArray = new SinTDArray();
            keyArray.ElementType = SinTDInstance.GetSinTDType("i32");
            SinTDMap SinTDMap = new SinTDMap(keyArray, SinTDInstance.GetSinTDType("string"));

            Dictionary<int[], string> nativeDictionary = (Dictionary<int[], string>)SinTDMap
                .AssignValuesToNativeType(arrayKeyDictionary, arrayKeyDictionary.GetType());
            Assert.AreEqual(arrayKeyDictionary.Keys, nativeDictionary.Keys);
            Assert.AreEqual(arrayKeyDictionary.Values, nativeDictionary.Values);
        }

        [Test()]
        public void ShouldAssignArrayTypedValuesToMap()
        {
            var arrayKeyDictionary = new Dictionary<string, int[]>{
                {"test", new int[]{1} }
            };

            SinTDArray valueArray = new SinTDArray();
            valueArray.ElementType = SinTDInstance.GetSinTDType("i32");
            SinTDMap SinTDMap = new SinTDMap(SinTDInstance.GetSinTDType("string"), valueArray);

            Dictionary<object, object> mapInstance
                = SinTDMap.AssignValuesFromObject(arrayKeyDictionary) as Dictionary<object, object>;

            Assert.AreEqual(typeof(object[]), mapInstance.Values.ElementAt(0).GetType());
            Assert.AreEqual(1, ((object[])mapInstance.Values.ElementAt(0)).Length);
            Assert.AreEqual(1, ((object[])mapInstance.Values.ElementAt(0))[0]);
        }

        [Test()]
        public void ShouldThrowExceptionOnWrongKeyType()
        {
            var baseTypeDictionary = new Dictionary<string, int> {
                {"test", 1}
            };

            SinTDMap SinTDMap = new SinTDMap(SinTDInstance.GetSinTDType("i32"), SinTDInstance.GetSinTDType("i32"));

            Assert.Throws<TypeCastException>(
                () => { var mapInstance
                    = SinTDMap.AssignValuesFromObject(baseTypeDictionary) as Dictionary<object, object>; });
        }

        [Test()]
        public void ShouldThrowExceptionOnWrongValueType()
        {
            var baseTypeDictionary = new Dictionary<string, int> {
                {"test", 1}
            };

            SinTDMap SinTDMap = new SinTDMap(SinTDInstance.GetSinTDType("string"), SinTDInstance.GetSinTDType("string"));

            Assert.Throws<TypeCastException>(
                () => { var mapInstance
                    = SinTDMap.AssignValuesFromObject(baseTypeDictionary) as Dictionary<object, object>; });
        }

        [Test()]
        public void ShouldAssignStructOfBaseTypesByName()
        {
            var structInstance = new testStruct{
                x = 1,
                y = 2
            };

            Dictionary<string, object> inst
                = intStruct.AssignValuesFromObject(structInstance) as Dictionary<string, object>;
            Assert.AreEqual(1, inst["x"]);
            Assert.AreEqual(2, inst["y"]);
        }

        [Test()]
        public void ShouldAssignStructToNativeStruct()
        {
            var structInstance = new testStruct
            {
                x = 1,
                y = 2
            };

            Dictionary<string, object> inst
                = intStruct.AssignValuesFromObject(structInstance) as Dictionary<string, object>;
            testStruct nativeStruct = (testStruct)intStruct.AssignValuesToNativeType(inst, typeof(testStruct));
            Assert.AreEqual(1, nativeStruct.x);
            Assert.AreEqual(2, nativeStruct.y);
        }

        [Test()]
        public void ShouldAssignStructWithArrayByName()
        {
            var aStructInstance = new arrayStruct {
                arr = new int[] {1}
            };

            Dictionary<string, object> inst
                = aStruct.AssignValuesFromObject(aStructInstance) as Dictionary<string, object>;
            object[] arrayInst = inst["arr"] as object[];
            Assert.AreEqual(1, arrayInst.Length);
            Assert.AreEqual(1, arrayInst[0]);
        }

        [Test()]
        public void ShouldAssignArrayStructInstanceToNativeArrayStruct()
        {
            var aStructInstance = new arrayStruct
            {
                arr = new int[] { 1 }
            };

            Dictionary<string, object> inst
                = aStruct.AssignValuesFromObject(aStructInstance) as Dictionary<string, object>;
            arrayStruct nativeStruct = (arrayStruct)aStruct.AssignValuesToNativeType(inst, typeof(arrayStruct));
            Assert.AreEqual(1, nativeStruct.arr.Length);
            Assert.AreEqual(1, nativeStruct.arr[0]);
        }

        [Test()]
        public void ShouldAssignStructWithMapByName()
        {
            var mStructInstance = new mapStruct
            {
                map = new Dictionary<string, bool> {
                    {"value", true}
                }
            };

            Dictionary<string, object> inst
                = mStruct.AssignValuesFromObject(mStructInstance) as Dictionary<string, object>;
            Dictionary<object, object> m = inst["map"] as Dictionary<object, object>;
            Assert.AreEqual("value", m.Keys.ElementAt(0));
            Assert.AreEqual(true, m.Values.ElementAt(0));
        }

        [Test()]
        public void ShouldAssignStructWithStructFromStructByName()
        {
            var structStructInstance = new structStruct
            {
                child = new testStruct
                {
                    x = 1,
                    y = 2,
                }
            };

            Dictionary<string, object> inst
                = sStruct.AssignValuesFromObject(structStructInstance) as Dictionary<string, object>;

            Dictionary<string, object> childInst = inst["child"] as Dictionary<string, object>;
            Assert.AreEqual(1, childInst["x"]);
            Assert.AreEqual(2, childInst["y"]);
        }

        [Test()]
        public void ShouldAssignStructFromClassWithFieldsByName()
        {
            var fieldClassInstance = new testClassFields
            {
                x = 1,
                y = 2
            };

            Dictionary<string, object> inst
                = intStruct.AssignValuesFromObject(fieldClassInstance) as Dictionary<string, object>;

            Assert.AreEqual(1, inst["x"]);
            Assert.AreEqual(2, inst["y"]);
        }

        [Test()]
        public void ShouldAssignStructFromClassWithPropertiesByName()
        {
            var propsClassInstance = new testClassProps
            {
                x = 1,
                y = 2
            };

            Dictionary<string, object> inst
                = intStruct.AssignValuesFromObject(propsClassInstance) as Dictionary<string, object>;

            Assert.AreEqual(1, inst["x"]);
            Assert.AreEqual(2, inst["y"]);
        }

        [Test()]
        public void ShouldAssignStructFromClassWithBothByName()
        {
            var bothClassInstance = new testClassBoth
            {
                x = 1,
                y = 2
            };

            Dictionary<string, object> inst
                = intStruct.AssignValuesFromObject(bothClassInstance) as Dictionary<string, object>;
            Assert.AreEqual(1, inst["x"]);
            Assert.AreEqual(2, inst["y"]);
        }

        [Test()]
        public void ShouldAssignStructInstanceToNativeClassWithBoth()
        {
            var bothClassInstance = new testClassBoth
            {
                x = 1,
                y = 2
            };

            Dictionary<string, object> inst
                = intStruct.AssignValuesFromObject(bothClassInstance) as Dictionary<string, object>;
            testClassBoth nativeClass = (testClassBoth)intStruct.AssignValuesToNativeType(inst, typeof(testClassBoth));
            Assert.AreEqual(1, nativeClass.x);
            Assert.AreEqual(2, nativeClass.y);
        }
    }
}
