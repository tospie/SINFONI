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
                && KTD.Instance.GetKtdType("i16").Name == "i16"
                && KTD.Instance.GetKtdType("i16").canBeAssignedFromType(typeof(Int16)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfInt32()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("i32")
                && KTD.Instance.GetKtdType("i32").Name == "i32"
                && KTD.Instance.GetKtdType("i32").canBeAssignedFromType(typeof(int)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfInt64()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("i64")
                && KTD.Instance.GetKtdType("i64").Name == "i64"
                && KTD.Instance.GetKtdType("i64").canBeAssignedFromType(typeof(Int64)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt16()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("u16")
                && KTD.Instance.GetKtdType("u16").Name == "u16"
                && KTD.Instance.GetKtdType("u16").canBeAssignedFromType(typeof(UInt16)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt32()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("u32")
                && KTD.Instance.GetKtdType("u32").Name == "u32"
                && KTD.Instance.GetKtdType("u32").canBeAssignedFromType(typeof(UInt32)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt64()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("u64")
                && KTD.Instance.GetKtdType("u64").Name == "u64"
                && KTD.Instance.GetKtdType("u64").canBeAssignedFromType(typeof(UInt64)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfBoolean()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("boolean")
                && KTD.Instance.GetKtdType("boolean").Name == "boolean"
                && KTD.Instance.GetKtdType("boolean").canBeAssignedFromType(typeof(bool)));
        }

        [Test()]
        public void ShouldCOntainCorrectDefinitionOfFloat()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("float")
                && KTD.Instance.GetKtdType("float").Name == "float"
                && KTD.Instance.GetKtdType("float").canBeAssignedFromType(typeof(float)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfString()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("string")
                && KTD.Instance.GetKtdType("string").Name == "string"
                && KTD.Instance.GetKtdType("string").canBeAssignedFromType(typeof(string)));
        }

        [Test()]
        public void ShouldContainDefinitionOfAny()
        {
            Assert.IsTrue(KTD.Instance.ContainsType("any")
                && KTD.Instance.GetKtdType("any").Name == "any");
        }

        [Test()]
        public void ShouldThrowExceptionOnRegisteringSameTypenameTwice()
        {
            Assert.DoesNotThrow(() => { KTD.Instance.RegisterType(new KtdType("newType")); });
            Assert.Throws(typeof(TypeNameConflictException),
                () => { KTD.Instance.RegisterType(new KtdType("newType")); }
                );
        }

        [Test()]
        public void ShouldThrowExceptionOnRequestingNonExistantType()
        {
            Assert.Throws(typeof(TypeNotRegisteredException),
                () => KTD.Instance.GetKtdType("notRegistered"));
        }

        [Test()]
        public void ShouldReturnRequestedType()
        {
            KtdType requestedType = KTD.Instance.GetKtdType("i16");
            Assert.AreEqual("i16", requestedType.Name);
        }
    }
}
