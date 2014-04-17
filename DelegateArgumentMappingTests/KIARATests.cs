using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit;
using NUnit.Framework;

namespace DelegateArgumentMappingTests
{
    [TestFixture()]
    class KIARATests
    {
        KtdType i32;
        KtdType ktd_string;
        KtdType ktd_bool;

        KtdType intStruct;
        KtdType nestedStruct;

        [SetUp()]
        public void InitTests()
        {

            i32 = new KtdType();
            i32.name = "i32";

            ktd_string = new KtdType();
            ktd_string.name = "string";

            ktd_bool = new KtdType();
            ktd_bool.name = "boolean";

            intStruct = new KtdType();
            intStruct.members["x"] = i32;
            intStruct.members["y"] = i32;
            intStruct.name = "intStruct";

            nestedStruct = new KtdType();
            nestedStruct.members["name"] = ktd_string;
            nestedStruct.members["b"] = ktd_bool;
            nestedStruct.members["s"] = intStruct;
        }

        [Test()]
        public void ShouldAssignToInt16()
        {
            KtdType i16 = new KtdType();
            i16.name = "i16";

            short value = 1;
            Assert.IsTrue(i16.canBeAssignedFromType(value.GetType()));
        }
        
        [Test()]
        public void ShouldAssignToInt32()
        {
            KtdType i32 = new KtdType();
            i32.name = "i32";            
            int valueInt = 1;

            Assert.IsTrue(i32.canBeAssignedFromType(valueInt.GetType()));
        }

        [Test()]
        public void ShouldAssignToInt64()
        {
            KtdType i64 = new KtdType();
            i64.name = "i64";
            long valueInt = 1;
            
            Assert.IsTrue(i64.canBeAssignedFromType(valueInt.GetType()));
        }

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

        [Test()]
        public void CanMapStruct()
        {
            var s = new testStruct { x = 1, affe = true};

            Assert.IsTrue(intStruct.canBeAssignedFromType(s.GetType()));
        }

        [Test()]
        public void CanMapStructsOfStructs()
        {
            var s = new testStruct { x = 1, affe = true };
            var cs = new nestedTestStruct { name = "me", b = true, s = s };

            Assert.IsTrue(nestedStruct.canBeAssignedFromType(typeof(nestedTestStruct)));
        }

        class testClass
        {
            public int x { get; set; }
            public int y { get; set; }
        }
    
        [Test()]
        public void CanMapClassWithProperties()
        {
            var c = new testClass { y = 2 };
            Assert.IsTrue(intStruct.canBeAssignedFromType(c.GetType()));
        }

        class nestedTestClass
        {
            public string name { get; set; }
            public bool b;

            public testClass s;
        }

        [Test()]
        public void CanAssignNestedClass()
        {
            var c = new nestedTestClass();
            c.s = new testClass();

            Assert.IsTrue(nestedStruct.canBeAssignedFromType(c.GetType()));
        }

        [Test()]
        public void CanAssignList()
        {
            var testList = new List<int>();
            int[] testListArray = null;
            var elementType = testList.GetType().GetProperty("Item").PropertyType;
            if(elementType == typeof(int))                        
                testListArray = checkIfList<int>(testList) as int[];

            Assert.IsNotNull(testListArray);
            Assert.IsTrue(typeof(Array).IsAssignableFrom(testList.ToArray().GetType()));
        }
        
        private object checkIfList<T>(object argument)        
        {
            if (argument.GetType() == typeof(List<T>))
            {
                List<T> castedArg  = (List<T>)argument;
                return castedArg.ToArray();
            }

            return null;
        }

        [Test()]
        public void ShouldMatchArrayTypes()
        {
            KtdArray ktd_array = new KtdArray();
            ktd_array.name = "array<int32>";
            ktd_array.elementType = i32;

            Assert.IsTrue(ktd_array.canBeAssignedFromType(typeof(int[])));
            Assert.IsTrue(ktd_array.canBeAssignedFromType(typeof(List<int>)));
            Assert.IsTrue(ktd_array.canBeAssignedFromType(typeof(ISet<int>)));            
        }
    }
}
