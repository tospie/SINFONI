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
        KtdStruct intStruct;
        KtdStruct aStruct;
        KtdStruct mStruct;
        KtdStruct sStruct;

        KtdType i32;
        KtdType ktdString;
        KtdType ktdBool;

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

        KTD ktdInstance = new KTD();

        [SetUp()]
        public void TestSetUp()
        {
            i32 = ktdInstance.GetKtdType("i32");
            ktdString = ktdInstance.GetKtdType("string");
            ktdBool = ktdInstance.GetKtdType("boolean");

            intStruct = new KtdStruct("intStruct");
            intStruct.members["x"] = i32;
            intStruct.members["y"] = i32;

            aStruct = new KtdStruct("arrayStruct");
            aStruct.members.Add("arr", new KtdArray(i32));
            ktdInstance.RegisterType(aStruct);

            mStruct = new KtdStruct("mapStruct");
            mStruct.members.Add("map", new KtdMap(ktdString, ktdBool));
            ktdInstance.RegisterType(mStruct);

            sStruct = new KtdStruct("structStruct");
            sStruct.members.Add("child", intStruct);
            ktdInstance.RegisterType(sStruct);

            ktdInstance = new KTD();
        }

        [Test()]
        public void ShouldAssignValueToInt16()
        {
            KtdType i16 = ktdInstance.GetKtdType("i16");
            short shortInstance = (short)i16.AssignValuesFromObject((short)1);
            Assert.AreEqual((short)1, shortInstance);
        }

        [Test()]
        public void ShouldAssingI16toNativeShort()
        {
            KtdType i16 = ktdInstance.GetKtdType("i16");
            Assert.AreEqual((short)1, i16.AssignValuesToNativeType(1, typeof(short)));
        }

        [Test()]
        public void ShouldAssignValueToUInt16()
        {
            KtdType u16 = ktdInstance.GetKtdType("u16");
            ushort ushortInstance = (ushort)u16.AssignValuesFromObject((ushort)1);
            Assert.AreEqual(1, ushortInstance);
        }

        [Test()]
        public void ShouldAssignUInt16ValueToNativeUshort()
        {
            KtdType u16 = ktdInstance.GetKtdType("u16");
            Assert.AreEqual(1, u16.AssignValuesToNativeType(1, typeof(ushort)));
        }

        [Test()]
        public void ShouldAssignValueToInt32()
        {
            KtdType i32 = ktdInstance.GetKtdType("i32");
            int intInstance = (int)i32.AssignValuesFromObject(1);
            Assert.AreEqual(1, intInstance);
        }

        [Test()]
        public void ShouldAssignInt32ToNativeInt()
        {
            KtdType i32 = ktdInstance.GetKtdType("i32");
            Assert.AreEqual(1, i32.AssignValuesToNativeType(1, typeof(int)));
        }

        [Test()]
        public void ShouldAssignValueToUInt32()
        {
            KtdType u32 = ktdInstance.GetKtdType("u32");
            uint uintInstance = (uint)u32.AssignValuesFromObject((uint)1);
            Assert.AreEqual(1, uintInstance);
        }

        [Test()]
        public void ShouldAssignUInt32ToNativeUInt()
        {
            KtdType u32 = ktdInstance.GetKtdType("u32");
            Assert.AreEqual(1, u32.AssignValuesToNativeType(1, typeof(uint)));
        }

        [Test()]
        public void ShouldAssignValueToInt64()
        {
            KtdType i64 = ktdInstance.GetKtdType("i64");
            long int64Instance = (long)i64.AssignValuesFromObject((long)1);
            Assert.AreEqual((long)1, int64Instance);
        }

        [Test()]
        public void ShouldAssignInt64ToNativeLong()
        {
            KtdType i64 = ktdInstance.GetKtdType("i64");
            Assert.AreEqual((long)1, i64.AssignValuesToNativeType(1, typeof(long)));
        }

        [Test()]
        public void ShouldAssignValueToUInt64()
        {
            KtdType u64 = ktdInstance.GetKtdType("u64");
            ulong u64Instance = (ulong)u64.AssignValuesFromObject((ulong)1);
            Assert.AreEqual((ulong)1, u64Instance);
        }

        [Test()]
        public void ShouldAssignU64ToNativeUlong()
        {
            KtdType u64 = ktdInstance.GetKtdType("u64");
            Assert.AreEqual((ulong)1, u64.AssignValuesToNativeType(1, typeof(ulong)));
        }

        [Test()]
        public void ShouldAssignValueToFloat()
        {
            KtdType f = ktdInstance.GetKtdType("float");
            float fInstance = (float)f.AssignValuesFromObject(1.0f);
            Assert.AreEqual(1.0f, fInstance);
        }

        [Test()]
        public void ShouldAssignFloatToNativeFloat()
        {
            KtdType f = ktdInstance.GetKtdType("float");
           Assert.AreEqual(1.0f, f.AssignValuesToNativeType(1.0, typeof(float)));
        }

        [Test()]
        public void ShouldAssignValueToDouble()
        {
            KtdType d = ktdInstance.GetKtdType("double");
            double dInstance = (double)d.AssignValuesFromObject(1.0);
            Assert.AreEqual(1.0, dInstance);
        }

        [Test()]
        public void ShouldAssignDoubleToNativeDouble()
        {
            KtdType d = ktdInstance.GetKtdType("double");
            Assert.AreEqual(1.0, d.AssignValuesToNativeType(1.0, typeof(double)));
        }

        [Test()]
        public void ShouldAssignBooleanToNativeBool()
        {
            KtdType b = ktdInstance.GetKtdType("boolean");
            Assert.AreEqual(true, b.AssignValuesToNativeType(true, typeof(bool)));
        }

        [Test()]
        public void ShouldAssignStringToNativeString()
        {
            KtdType s = ktdInstance.GetKtdType("string");
            Assert.AreEqual("Hello World", s.AssignValuesToNativeType("Hello World", typeof(string)));
        }

        [Test()]
        public void ShouldAssignValueToAny()
        {
            KtdType any = ktdInstance.GetKtdType("any");
            var anyValue = 1;

            var stringInstance = any.AssignValuesFromObject(anyValue);
            Assert.AreEqual(anyValue, stringInstance);
        }

        [Test()]
        public void ShouldAssignAnyToNative()
        {
            KtdType any = ktdInstance.GetKtdType("any");
            var anyValue = 1;

            Assert.AreEqual(anyValue, any.AssignValuesToNativeType(anyValue, anyValue.GetType()));
        }

        [Test()]
        public void ShouldAssignArrayToArray()
        {
            var intArray = new int[] { 1 };
            KtdArray array = new KtdArray();
            array.elementType = ktdInstance.GetKtdType("i32");

            var arrayInstance =  array.AssignValuesFromObject(intArray) as object[];
            Assert.AreEqual(1, arrayInstance.Length);
            Assert.AreEqual(1, (int)arrayInstance[0]);
        }

        [Test()]
        public void ShouldAssignArrayToNativeArray()
        {
            var intArray = new int[0];
            KtdArray array = new KtdArray();
            array.elementType = ktdInstance.GetKtdType("i32");
            var test = array.AssignValuesToNativeType(new int[1]{1}, intArray.GetType());
            Assert.AreEqual(1, ((int[])test).Length);
            Assert.AreEqual(1, ((int[])test)[0]);
        }

        [Test()]
        public void ShouldAssignListToArray()
        {
            var intList = new List<int>{ 1 };
            KtdArray array = new KtdArray();
            array.elementType = ktdInstance.GetKtdType("i32");

            var arrayInstance = array.AssignValuesFromObject(intList) as object[];
            Assert.AreEqual(1, arrayInstance.Length);
            Assert.AreEqual(1, (int)arrayInstance[0]);
        }

        [Test()]
        public void ShouldAssignArrayToNativeList()
        {
            var intList = new List<int>();
            KtdArray array = new KtdArray();
            array.elementType = ktdInstance.GetKtdType("i32");
            List<int> test = (List<int>)array.AssignValuesToNativeType(new int[1] { 1 }, intList.GetType());
            Assert.AreEqual(1, test.Count);
            Assert.AreEqual(1, test[0]);
        }

        [Test()]
        public void ShouldAssignSetToArray()
        {
            var intSet = new HashSet<int> { 1 };
            KtdArray array = new KtdArray();
            array.elementType = ktdInstance.GetKtdType("i32");

            object[] arrayInstance = array.AssignValuesFromObject(intSet) as object[];
            Assert.AreEqual(1, arrayInstance.Length);
            Assert.AreEqual(1, (int)arrayInstance[0]);
        }

        [Test()]
        public void ShouldAssignArrayToNativeSet()
        {
            var intSet = new HashSet<int>();
            KtdArray array = new KtdArray();
            array.elementType = ktdInstance.GetKtdType("i32");
            ISet<int> test = (HashSet<int>)array.AssignValuesToNativeType(new int[1] { 1 }, intSet.GetType());
            Assert.AreEqual(1, test.Count);
            Assert.AreEqual(1, test.ElementAt(0));
        }

        [Test()]
        public void ShouldAssignArrayOfArraysToNative()
        {
            var intArray = new int[1][];
            intArray[0] = new int[1] { 2 };
            KtdArray innerArray = new KtdArray();
            innerArray.elementType = ktdInstance.GetKtdType("i32");
            KtdArray outerArray = new KtdArray();
            outerArray.elementType = innerArray;
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
            KtdArray innerArray = new KtdArray();
            innerArray.elementType = ktdInstance.GetKtdType("i32");
            KtdArray outerArray = new KtdArray();
            outerArray.elementType = innerArray;
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
            KtdArray innerArray = new KtdArray();
            innerArray.elementType = ktdInstance.GetKtdType("i32");
            KtdArray outerArray = new KtdArray();
            outerArray.elementType = innerArray;
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
            KtdArray innerArray = new KtdArray();
            innerArray.elementType = ktdInstance.GetKtdType("i32");
            KtdArray outerArray = new KtdArray();
            outerArray.elementType = innerArray;
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
            KtdArray innerArray = new KtdArray();
            innerArray.elementType = ktdInstance.GetKtdType("i32");
            KtdArray outerArray = new KtdArray();
            outerArray.elementType = innerArray;
            ISet<int[]> test = (HashSet<int[]>)outerArray.AssignValuesToNativeType(intArray, arraySet.GetType());
            Assert.IsAssignableFrom<int[]>(test.ElementAt(0));
            Assert.AreEqual(2, test.ElementAt(0)[0]);
        }

        [Test()]
        public void ShouldAssignEmptyArray()
        {
            var intArray = new int[] {};
            KtdArray array = new KtdArray();
            array.elementType = ktdInstance.GetKtdType("i32");

            object[] arrayInstance = array.AssignValuesFromObject(intArray) as object[];
            Assert.AreEqual(0, arrayInstance.Length);
        }

        [Test()]
        public void ShouldThrowExceptionWhenAssigningNonEnumerableToArray()
        {
            KtdArray array = new KtdArray();
            array.elementType = ktdInstance.GetKtdType("i32");

            Assert.Throws<TypeCastException>(
                () => { object[] arrayInstance = array.AssignValuesFromObject(1) as object[]; });
        }

        [Test()]
        public void ShouldThrowExceptionWhenAssigningArrayWithWrongElementType()
        {
            var floatArray = new float[] { 1f };
            KtdArray array = new KtdArray();
            array.elementType = ktdInstance.GetKtdType("i32");

            Assert.Throws<TypeCastException>(
                () => { var arrayInstance = array.AssignValuesFromObject(floatArray) as object[]; });
        }

        [Test()]
        public void ShouldAssignDictionaryOfBaseTypesToMap()
        {
            var baseTypeDictionary = new Dictionary<string, int> {
                {"test", 1}
            };

            KtdMap ktdMap = new KtdMap();
            ktdMap.keyType = ktdInstance.GetKtdType("string");
            ktdMap.elementType = ktdInstance.GetKtdType("i32");

            Dictionary<object, object> mapInstance
                = ktdMap.AssignValuesFromObject(baseTypeDictionary) as Dictionary<object, object>;

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

            KtdMap ktdMap = new KtdMap();
            ktdMap.keyType = ktdInstance.GetKtdType("string");
            ktdMap.elementType = ktdInstance.GetKtdType("i32");
            Dictionary<string, int> nativeDictionary =
                (Dictionary<string, int>)ktdMap.AssignValuesToNativeType(baseTypeDictionary,
                typeof(Dictionary<string, int>));
            Assert.AreEqual(baseTypeDictionary, nativeDictionary);
        }

        [Test()]
        public void ShouldAssignedArrayTypedKeysToMap()
        {
            var arrayKeyDictionary = new Dictionary<int[], string>{
                {new int[]{1}, "test"}
            };

            KtdArray keyArray = new KtdArray();
            keyArray.elementType = ktdInstance.GetKtdType("i32");
            KtdMap ktdMap = new KtdMap();

            ktdMap.keyType = keyArray;
            ktdMap.elementType = ktdInstance.GetKtdType("string");
            Dictionary<object, object> mapInstance
                = ktdMap.AssignValuesFromObject(arrayKeyDictionary) as Dictionary<object, object>;

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

            KtdArray keyArray = new KtdArray();
            keyArray.elementType = ktdInstance.GetKtdType("i32");
            KtdMap ktdMap = new KtdMap();

            ktdMap.keyType = keyArray;
            ktdMap.elementType = ktdInstance.GetKtdType("string");
            Dictionary<int[], string> nativeDictionary = (Dictionary<int[], string>)ktdMap
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

            KtdArray valueArray = new KtdArray();
            valueArray.elementType = ktdInstance.GetKtdType("i32");
            KtdMap ktdMap = new KtdMap();

            ktdMap.keyType =  ktdInstance.GetKtdType("string");
            ktdMap.elementType = valueArray;
            Dictionary<object, object> mapInstance
                = ktdMap.AssignValuesFromObject(arrayKeyDictionary) as Dictionary<object, object>;

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

            KtdMap ktdMap = new KtdMap();
            ktdMap.keyType = ktdInstance.GetKtdType("i32");
            ktdMap.elementType = ktdInstance.GetKtdType("i32");

            Assert.Throws<TypeCastException>(
                () => { var mapInstance
                    = ktdMap.AssignValuesFromObject(baseTypeDictionary) as Dictionary<object, object>; });
        }

        [Test()]
        public void ShouldThrowExceptionOnWrongValueType()
        {
            var baseTypeDictionary = new Dictionary<string, int> {
                {"test", 1}
            };

            KtdMap ktdMap = new KtdMap();
            ktdMap.keyType = ktdInstance.GetKtdType("string");
            ktdMap.elementType = ktdInstance.GetKtdType("string");

            Assert.Throws<TypeCastException>(
                () => { var mapInstance
                    = ktdMap.AssignValuesFromObject(baseTypeDictionary) as Dictionary<object, object>; });
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
