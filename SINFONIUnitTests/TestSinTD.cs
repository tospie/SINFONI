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
    class TestSinTD
    {
        SinTD SinTDInstance = new SinTD();
        [Test()]
        public void ShouldContainCorrectDefinitionOfInt16()
        {
            Assert.IsTrue(SinTDInstance.ContainsType("i16")
                && SinTDInstance.GetSinTDType("i16").Name == "i16"
                && SinTDInstance.GetSinTDType("i16").CanBeAssignedFromType(typeof(Int16)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfInt32()
        {
            Assert.IsTrue(SinTDInstance.ContainsType("i32")
                && SinTDInstance.GetSinTDType("i32").Name == "i32"
                && SinTDInstance.GetSinTDType("i32").CanBeAssignedFromType(typeof(int)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfInt64()
        {
            Assert.IsTrue(SinTDInstance.ContainsType("i64")
                && SinTDInstance.GetSinTDType("i64").Name == "i64"
                && SinTDInstance.GetSinTDType("i64").CanBeAssignedFromType(typeof(Int64)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt16()
        {
            Assert.IsTrue(SinTDInstance.ContainsType("u16")
                && SinTDInstance.GetSinTDType("u16").Name == "u16"
                && SinTDInstance.GetSinTDType("u16").CanBeAssignedFromType(typeof(UInt16)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt32()
        {
            Assert.IsTrue(SinTDInstance.ContainsType("u32")
                && SinTDInstance.GetSinTDType("u32").Name == "u32"
                && SinTDInstance.GetSinTDType("u32").CanBeAssignedFromType(typeof(UInt32)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfUInt64()
        {
            Assert.IsTrue(SinTDInstance.ContainsType("u64")
                && SinTDInstance.GetSinTDType("u64").Name == "u64"
                && SinTDInstance.GetSinTDType("u64").CanBeAssignedFromType(typeof(UInt64)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfBoolean()
        {
            Assert.IsTrue(SinTDInstance.ContainsType("boolean")
                && SinTDInstance.GetSinTDType("boolean").Name == "boolean"
                && SinTDInstance.GetSinTDType("boolean").CanBeAssignedFromType(typeof(bool)));
        }

        [Test()]
        public void ShouldCOntainCorrectDefinitionOfFloat()
        {
            Assert.IsTrue(SinTDInstance.ContainsType("float")
                && SinTDInstance.GetSinTDType("float").Name == "float"
                && SinTDInstance.GetSinTDType("float").CanBeAssignedFromType(typeof(float)));
        }

        [Test()]
        public void ShouldContainCorrectDefinitionOfString()
        {
            Assert.IsTrue(SinTDInstance.ContainsType("string")
                && SinTDInstance.GetSinTDType("string").Name == "string"
                && SinTDInstance.GetSinTDType("string").CanBeAssignedFromType(typeof(string)));
        }

        [Test()]
        public void ShouldContainDefinitionOfAny()
        {
            Assert.IsTrue(SinTDInstance.ContainsType("any")
                && SinTDInstance.GetSinTDType("any").Name == "any");
        }

        [Test()]
        public void ShouldThrowExceptionOnRegisteringSameTypenameTwice()
        {
            Assert.DoesNotThrow(() => { SinTDInstance.RegisterType(new SinTDType("newType")); });
            Assert.Throws(typeof(TypeNameConflictException),
                () => { SinTDInstance.RegisterType(new SinTDType("newType")); }
                );
        }

        [Test()]
        public void ShouldThrowExceptionOnRequestingNonExistantType()
        {
            Assert.Throws(typeof(TypeNotRegisteredException),
                () => SinTDInstance.GetSinTDType("notRegistered"));
        }

        [Test()]
        public void ShouldThrowExceptionOnInvalidTypeName()
        {
            Assert.Throws<InvalidTypeNameException>(() => SinTDInstance.RegisterType(new SinTDType()));
            Assert.Throws<InvalidTypeNameException>(() => SinTDInstance.RegisterType(new SinTDType("")));
        }

        [Test()]
        public void ShouldReturnRequestedType()
        {
            SinTDType requestedType = SinTDInstance.GetSinTDType("i16");
            Assert.AreEqual("i16", requestedType.Name);
        }
    }
}
