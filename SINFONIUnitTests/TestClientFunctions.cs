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
    class TestConnection : Connection {

        public new void Disconnect()
        {
            throw new NotImplementedException();
        }

        protected new void RegisterHandler(string funcName, Delegate handler)
        {
            throw new NotImplementedException();
        }

        protected override IClientFunctionCall CallClientFunction(string funcName, params object[] args)
        {
            IClientFunctionCall mockCall = null;
            return mockCall;
        }
    }

    [TestFixture()]
    class TestClientFunctions
    {
        SINFONIService service;
        ServiceFunctionDescription serviceFunction;

        SinTDType i32;
        SinTDType SinTD_string;
        SinTDStruct intStruct;

        TestConnection connection;

        SinTD SinTDInstance;
        struct testStruct
        {
            public int x;
            public int y;
            public bool affe;
        }

        [TestFixtureSetUp()]
        public void Setup()
        {
            SinTDInstance = new SinTD();
            SinTDInstance.SINFONIServices = new ServiceRegistry();

            i32 = SinTDInstance.GetSinTDType("i32");
            SinTD_string = SinTDInstance.GetSinTDType("string");

            intStruct = new SinTDStruct("intStruct");
            intStruct.AddMember("x", i32);
            intStruct.AddMember("y", i32);

            serviceFunction = new ServiceFunctionDescription("function", new SinTDType("void"));
            serviceFunction.Parameters.Add("intParameter", i32);
            serviceFunction.Parameters.Add("stringParameter", SinTD_string);

            service = new SINFONIService("service");
            service.serviceFunctions.Add("function", serviceFunction);

            SinTDInstance.SINFONIServices.services.Add("service", service);
            connection = new TestConnection();
            connection.SinTD = SinTDInstance;
        }
        
        [Test()]
        public void CallShouldBeValidForCorrectParameters()
        {
            var clientFunction = connection.GenerateClientFunction("service", "function");
            Assert.DoesNotThrow(() => clientFunction(1, "Hello World"));
        }


        [Test()]
        public void CallShouldBeValidForArrayParameters()
        {
            SinTDArray parameterArray = new SinTDArray();
            parameterArray.ElementType = i32;

            var serviceFunction = new ServiceFunctionDescription("arrayFunction", new SinTDType("void"));
            serviceFunction.Parameters.Add("arrayParam", parameterArray);
            service.serviceFunctions.Add("arrayFunction", serviceFunction);
            var clientFunction = connection.GenerateClientFunction("service", "arrayFunction");
            Assert.DoesNotThrow(() => clientFunction(new int[] {1, 2, 3, 4}));
        }

        [Test()]
        public void CallShouldBeValidForMapParameters()
        {
            SinTDMap parameterMap = new SinTDMap(SinTD_string, i32);

            var serviceFunction = new ServiceFunctionDescription("mapFunction", new SinTDType("void"));
            serviceFunction.Parameters.Add("mapParam", parameterMap);
            service.serviceFunctions.Add("mapFunction", serviceFunction);
            var clientFunction = connection.GenerateClientFunction("service", "mapFunction");
            Assert.DoesNotThrow(() => clientFunction(new Dictionary<string, int> {
                {"first", 1},
                {"second", 2}
            }));
        }

        [Test()]
        public void CallShouldBeValidForStructParameters()
        {
            var serviceFunction = new ServiceFunctionDescription("structFunction", new SinTDType("void"));
            serviceFunction.Parameters.Add("structParam", intStruct);
            service.serviceFunctions.Add("structFunction", serviceFunction);
            var clientFunction = connection.GenerateClientFunction("service", "structFunction");
            Assert.DoesNotThrow(() => clientFunction(new testStruct { x = 1, y = 1 }));
        }

        [Test()]
        public void CallShouldThrowExceptionWhenRegisteringToNonexistingService()
        {
            Assert.Throws<ServiceNotRegisteredException>(
                () => connection.GenerateClientFunction("invalid_service", "invalid_function"));
        }

        [Test()]
        public void CallShouldThrowExceptionWhenRegisteringToNonexistingServiceFunction()
        {
            Assert.Throws<ServiceNotRegisteredException>(
                () => connection.GenerateClientFunction("service", "invalid_function"));
        }

        [Test()]
        public void CallShouldThrowExceptionForWrongParameterCount()
        {
            var clientFunction = connection.GenerateClientFunction("service", "function");
            Assert.Throws<ParameterMismatchException>(() => clientFunction("Hello World"));
        }

        [Test()]
        public void CallShouldThrowExceptionForWrongParameterType()
        {
            var clientFunction = connection.GenerateClientFunction("service", "function");
            Assert.Throws<ParameterMismatchException>(() => clientFunction(1.0f, "Hello World"));
        }
    }
}
