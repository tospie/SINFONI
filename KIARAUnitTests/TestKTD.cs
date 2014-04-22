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
    class TestKTD
    {

        [Test()]
        public void ShouldContainCorrectDefinitionOfInt16()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("i16")
                && KTD.Instance.GetType("i16").Name == "i16"
                && KTD.Instance.GetType("i16").canBeAssignedFromType(typeof(Int16)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfInt32()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("i32")
                && KTD.Instance.GetType("i32").Name == "i32"
                && KTD.Instance.GetType("i32").canBeAssignedFromType(typeof(int)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfInt64()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("i64")
                && KTD.Instance.GetType("i64").Name == "i64"
                && KTD.Instance.GetType("i64").canBeAssignedFromType(typeof(Int64)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt16()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("u16")
                && KTD.Instance.GetType("u16").Name == "u16"
                && KTD.Instance.GetType("u16").canBeAssignedFromType(typeof(UInt16)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt32()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("u32")
                && KTD.Instance.GetType("u32").Name == "u32"
                && KTD.Instance.GetType("u32").canBeAssignedFromType(typeof(UInt32)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt64()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("u64")
                && KTD.Instance.GetType("u64").Name == "u64"
                && KTD.Instance.GetType("u64").canBeAssignedFromType(typeof(UInt64)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfBoolean()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("boolean")
                && KTD.Instance.GetType("boolean").Name == "boolean"
                && KTD.Instance.GetType("boolean").canBeAssignedFromType(typeof(bool)));
        }

        [Test()]
        public void ShouldCOntainCorrectDefinitionOfFloat()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("float")
                && KTD.Instance.GetType("float").Name == "float"
                && KTD.Instance.GetType("float").canBeAssignedFromType(typeof(float)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfString()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("string")
                && KTD.Instance.GetType("string").Name == "string"
                && KTD.Instance.GetType("string").canBeAssignedFromType(typeof(string)));
        }

        [Test()]
        public void ShouldContainDefinitionOfAny()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("any")
                && KTD.Instance.GetType("any").Name == "any");
        }

        [Test()]
        public void ShouldThrowExceptionOnRegisteringSameTypenameTwice()
        {
            Assert.DoesNotThrow(() => { KTD.Instance.RegisterType("newType", new KtdType()); });
            Assert.Throws(typeof(TypeNameConflictException),
                () => { KTD.Instance.RegisterType("newType", new KtdType()); }
                );
        }

        [Test()]
        public void ShouldThrowExceptionOnRequestingNonExistantType()
        {
            Assert.Throws(typeof(TypeNotRegisteredException),
                () => KTD.Instance.GetType("notRegistered"));
        }

        [Test()]
        public void ShouldReturnRequestedType()
        {
            KtdType requestedType = KTD.Instance.GetType("i16");
            Assert.AreEqual("i16", requestedType.Name);
        }
    }
}
