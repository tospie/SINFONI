using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SINFONI;
using SINFONI.Exceptions;
using NUnit.Framework;

namespace KIARAUnitTests
{
    [TestFixture()]
    class TestKTD
    {
        KTD ktdInstance = new KTD();
        [Test()]
        public void ShouldContainCorrectDefinitionOfInt16()
        {
            Assert.IsTrue(ktdInstance.ContainsType("i16")
                && ktdInstance.GetKtdType("i16").Name == "i16"
                && ktdInstance.GetKtdType("i16").CanBeAssignedFromType(typeof(Int16)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfInt32()
        {
            Assert.IsTrue(ktdInstance.ContainsType("i32")
                && ktdInstance.GetKtdType("i32").Name == "i32"
                && ktdInstance.GetKtdType("i32").CanBeAssignedFromType(typeof(int)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfInt64()
        {
            Assert.IsTrue(ktdInstance.ContainsType("i64")
                && ktdInstance.GetKtdType("i64").Name == "i64"
                && ktdInstance.GetKtdType("i64").CanBeAssignedFromType(typeof(Int64)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt16()
        {
            Assert.IsTrue(ktdInstance.ContainsType("u16")
                && ktdInstance.GetKtdType("u16").Name == "u16"
                && ktdInstance.GetKtdType("u16").CanBeAssignedFromType(typeof(UInt16)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt32()
        {
            Assert.IsTrue(ktdInstance.ContainsType("u32")
                && ktdInstance.GetKtdType("u32").Name == "u32"
                && ktdInstance.GetKtdType("u32").CanBeAssignedFromType(typeof(UInt32)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt64()
        {
            Assert.IsTrue(ktdInstance.ContainsType("u64")
                && ktdInstance.GetKtdType("u64").Name == "u64"
                && ktdInstance.GetKtdType("u64").CanBeAssignedFromType(typeof(UInt64)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfBoolean()
        {
            Assert.IsTrue(ktdInstance.ContainsType("boolean")
                && ktdInstance.GetKtdType("boolean").Name == "boolean"
                && ktdInstance.GetKtdType("boolean").CanBeAssignedFromType(typeof(bool)));
        }

        [Test()]
        public void ShouldCOntainCorrectDefinitionOfFloat()
        {
            Assert.IsTrue(ktdInstance.ContainsType("float")
                && ktdInstance.GetKtdType("float").Name == "float"
                && ktdInstance.GetKtdType("float").CanBeAssignedFromType(typeof(float)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfString()
        {
            Assert.IsTrue(ktdInstance.ContainsType("string")
                && ktdInstance.GetKtdType("string").Name == "string"
                && ktdInstance.GetKtdType("string").CanBeAssignedFromType(typeof(string)));
        }

        [Test()]
        public void ShouldContainDefinitionOfAny()
        {
            Assert.IsTrue(ktdInstance.ContainsType("any")
                && ktdInstance.GetKtdType("any").Name == "any");
        }

        [Test()]
        public void ShouldThrowExceptionOnRegisteringSameTypenameTwice()
        {
            Assert.DoesNotThrow(() => { ktdInstance.RegisterType(new KtdType("newType")); });
            Assert.Throws(typeof(TypeNameConflictException),
                () => { ktdInstance.RegisterType(new KtdType("newType")); }
                );
        }

        [Test()]
        public void ShouldThrowExceptionOnRequestingNonExistantType()
        {
            Assert.Throws(typeof(TypeNotRegisteredException),
                () => ktdInstance.GetKtdType("notRegistered"));
        }

        [Test()]
        public void ShouldThrowExceptionOnInvalidTypeName()
        {
            Assert.Throws<InvalidTypeNameException>(() => ktdInstance.RegisterType(new KtdType()));
            Assert.Throws<InvalidTypeNameException>(() => ktdInstance.RegisterType(new KtdType("")));
        }

        [Test()]
        public void ShouldReturnRequestedType()
        {
            KtdType requestedType = ktdInstance.GetKtdType("i16");
            Assert.AreEqual("i16", requestedType.Name);
        }
    }
}
