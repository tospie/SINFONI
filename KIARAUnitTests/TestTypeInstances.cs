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
    public class TestTypeInstances
    {
        [SetUp()]
        public void TestSetUp()
        {
            KTD.Instance = new KTD();
        }

        [Test()]
        public void ShouldAssignValueToInt16()
        {
            KtdType i16 = KTD.Instance.GetKtdType("i16");
            KtdTypeInstance shortInstance = i16.AssignValuesFromObject((short)1) as KtdTypeInstance;
            Assert.AreEqual((short)1, (short)shortInstance.Value);
        }

        [Test()]
        public void ShouldAssignValueToUInt16()
        {
            KtdType u16 = KTD.Instance.GetKtdType("u16");
            KtdTypeInstance ushortInstance = u16.AssignValuesFromObject((ushort)1) as KtdTypeInstance;
            Assert.AreEqual(1, (ushort)ushortInstance.Value);
        }

        [Test()]
        public void ShouldAssignValueToInt32()
        {
            KtdType i32 = KTD.Instance.GetKtdType("i32");
            KtdTypeInstance intInstance = i32.AssignValuesFromObject(1) as KtdTypeInstance;
            Assert.AreEqual(1, (int)intInstance.Value);
        }

        [Test()]
        public void ShouldAssignValueToUInt32()
        {
            KtdType u32 = KTD.Instance.GetKtdType("u32");
            KtdTypeInstance uintInstance = u32.AssignValuesFromObject((uint)1) as KtdTypeInstance;
            Assert.AreEqual(1, (uint)uintInstance.Value);
        }

        [Test()]
        public void ShouldAssignValueToInt64()
        {
            KtdType i64 = KTD.Instance.GetKtdType("i64");
            KtdTypeInstance int64Instance = i64.AssignValuesFromObject((long)1) as KtdTypeInstance;
            Assert.AreEqual((long)1, (long)int64Instance.Value);
        }

        [Test()]
        public void ShouldAssignValueToUInt64()
        {
            KtdType u64 = KTD.Instance.GetKtdType("u64");
            KtdTypeInstance u64Instance = u64.AssignValuesFromObject((ulong)1) as KtdTypeInstance;
            Assert.AreEqual((ulong)1, (ulong)u64Instance.Value);
        }

        [Test()]
        public void ShouldAssignValueToFloat()
        {
            KtdType f = KTD.Instance.GetKtdType("float");
            KtdTypeInstance fInstance = f.AssignValuesFromObject(1.0f) as KtdTypeInstance;
            Assert.AreEqual(1.0f, (float)fInstance.Value);
        }

        [Test()]
        public void ShouldAssignValueToDouble()
        {
            KtdType d = KTD.Instance.GetKtdType("double");
            KtdTypeInstance dInstance = d.AssignValuesFromObject(1.0) as KtdTypeInstance;
            Assert.AreEqual(1.0, (double)dInstance.Value);
        }

        [Test()]
        public void ShouldAssignValueToBoolean()
        {
            KtdType b = KTD.Instance.GetKtdType("boolean");
            KtdTypeInstance boolInstance = b.AssignValuesFromObject(true) as KtdTypeInstance;
            Assert.AreEqual(true, (bool)boolInstance.Value);
        }

        [Test()]
        public void ShouldAssignValueToString()
        {
            KtdType s = KTD.Instance.GetKtdType("string");
            KtdTypeInstance stringInstance = s.AssignValuesFromObject("Hello World") as KtdTypeInstance;
            Assert.AreEqual("Hello World", (string)stringInstance.Value);
        }

        [Test()]
        public void ShouldAssignValueToAny()
        {
            KtdType any = KTD.Instance.GetKtdType("any");
            var anyValue = 1;

            KtdTypeInstance stringInstance = any.AssignValuesFromObject(anyValue) as KtdTypeInstance;
            Assert.AreEqual(anyValue, stringInstance.Value);
        }

        [Test()]
        public void ShouldAssignArrayToArray()
        {
            var intArray = new int[] { 1 };
            KtdArray array = new KtdArray();
            array.elementType = KTD.Instance.GetKtdType("i32");

            KtdArrayInstance arrayInstance =  array.AssignValuesFromObject(intArray) as KtdArrayInstance;
            Assert.AreEqual(1, arrayInstance.Values.Length);
            Assert.AreEqual(1, (int)arrayInstance.Values[0].Value);
        }

        [Test()]
        public void ShouldAssignListToArray()
        {
            var intList = new List<int>{ 1 };
            KtdArray array = new KtdArray();
            array.elementType = KTD.Instance.GetKtdType("i32");

            KtdArrayInstance arrayInstance = array.AssignValuesFromObject(intList) as KtdArrayInstance;
            Assert.AreEqual(1, arrayInstance.Values.Length);
            Assert.AreEqual(1, (int)arrayInstance.Values[0].Value);
        }

        [Test()]
        public void ShouldAssignSetToArray()
        {
            var intSet = new HashSet<int> { 1 };
            KtdArray array = new KtdArray();
            array.elementType = KTD.Instance.GetKtdType("i32");

            KtdArrayInstance arrayInstance = array.AssignValuesFromObject(intSet) as KtdArrayInstance;
            Assert.AreEqual(1, arrayInstance.Values.Length);
            Assert.AreEqual(1, (int)arrayInstance.Values[0].Value);
        }

        [Test()]
        public void ShouldAssignEmptyArray()
        {
            var intArray = new int[] {};
            KtdArray array = new KtdArray();
            array.elementType = KTD.Instance.GetKtdType("i32");

            KtdArrayInstance arrayInstance = array.AssignValuesFromObject(intArray) as KtdArrayInstance;
            Assert.AreEqual(0, arrayInstance.Values.Length);
        }

        [Test()]
        public void ShouldThrowExceptionWhenAssigningNonEnumerableToArray()
        {
            KtdArray array = new KtdArray();
            array.elementType = KTD.Instance.GetKtdType("i32");

            Assert.Throws<TypeCastException>(
                () => { KtdArrayInstance arrayInstance = array.AssignValuesFromObject(1) as KtdArrayInstance; });
        }

        [Test()]
        public void ShouldThrowExceptionWhenAssigningArrayWithWrongElementType()
        {
            var floatArray = new float[] { 1f };
            KtdArray array = new KtdArray();
            array.elementType = KTD.Instance.GetKtdType("i32");

            Assert.Throws<TypeCastException>(
                () => { var arrayInstance = array.AssignValuesFromObject(floatArray) as KtdArrayInstance; });
        }

        [Test()]
        public void ShouldAssignDictionaryOfBaseTypesToMap()
        {
            var baseTypeDictionary = new Dictionary<string, int> {
                {"test", 1}
            };

            KtdMap ktdMap = new KtdMap();
            ktdMap.keyType = KTD.Instance.GetKtdType("string");
            ktdMap.elementType = KTD.Instance.GetKtdType("i32");

            KtdMapInstance mapInstance = ktdMap.AssignValuesFromObject(baseTypeDictionary) as KtdMapInstance;

            Assert.AreEqual(1, mapInstance.Values.Count);
            Assert.AreEqual("test", ((KtdTypeInstance)mapInstance.Values.Keys.ElementAt(0)).Value);
            Assert.AreEqual(1, ((KtdTypeInstance)mapInstance.Values.Values.ElementAt(0)).Value);
        }

        [Test()]
        public void ShouldAssignedArrayTypedKeysToMap()
        {
            var arrayKeyDictionary = new Dictionary<int[], string>{
                {new int[]{1}, "test"}
            };

            KtdArray keyArray = new KtdArray();
            keyArray.elementType = KTD.Instance.GetKtdType("i32");
            KtdMap ktdMap = new KtdMap();

            ktdMap.keyType = keyArray;
            ktdMap.elementType = KTD.Instance.GetKtdType("string");
            KtdMapInstance mapInstance = ktdMap.AssignValuesFromObject(arrayKeyDictionary) as KtdMapInstance;

            Assert.AreEqual(typeof(KtdArrayInstance), mapInstance.Values.Keys.ElementAt(0).GetType());
            Assert.AreEqual(1, ((KtdArrayInstance)mapInstance.Values.Keys.ElementAt(0)).Values.Length);
            Assert.AreEqual(1, ((KtdArrayInstance)mapInstance.Values.Keys.ElementAt(0)).Values[0].Value);
        }

        [Test()]
        public void ShouldAssignedArrayTypedValuesToMap()
        {
            var arrayKeyDictionary = new Dictionary<string, int[]>{
                {"test", new int[]{1} }
            };

            KtdArray valueArray = new KtdArray();
            valueArray.elementType = KTD.Instance.GetKtdType("i32");
            KtdMap ktdMap = new KtdMap();

            ktdMap.keyType =  KTD.Instance.GetKtdType("string");
            ktdMap.elementType = valueArray;
            KtdMapInstance mapInstance = ktdMap.AssignValuesFromObject(arrayKeyDictionary) as KtdMapInstance;

            Assert.AreEqual(typeof(KtdArrayInstance), mapInstance.Values.Values.ElementAt(0).GetType());
            Assert.AreEqual(1, ((KtdArrayInstance)mapInstance.Values.Values.ElementAt(0)).Values.Length);
            Assert.AreEqual(1, ((KtdArrayInstance)mapInstance.Values.Values.ElementAt(0)).Values[0].Value);
        }

        [Test()]
        public void ShouldThrowExceptionOnWrongKeyType()
        {
            var baseTypeDictionary = new Dictionary<string, int> {
                {"test", 1}
            };

            KtdMap ktdMap = new KtdMap();
            ktdMap.keyType = KTD.Instance.GetKtdType("i32");
            ktdMap.elementType = KTD.Instance.GetKtdType("i32");

            Assert.Throws<TypeCastException>(
                () => { var mapInstance = ktdMap.AssignValuesFromObject(baseTypeDictionary) as KtdMapInstance; });
        }

        [Test()]
        public void ShouldThrowExceptionOnWrongValueType()
        {
            var baseTypeDictionary = new Dictionary<string, int> {
                {"test", 1}
            };

            KtdMap ktdMap = new KtdMap();
            ktdMap.keyType = KTD.Instance.GetKtdType("string");
            ktdMap.elementType = KTD.Instance.GetKtdType("string");

            Assert.Throws<TypeCastException>(
                () => { var mapInstance = ktdMap.AssignValuesFromObject(baseTypeDictionary) as KtdMapInstance; });
        }
    }
}
