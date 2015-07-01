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

        KtdType i32;
        KtdType ktd_string;
        KtdStruct intStruct;

        TestConnection connection;

        KTD ktdInstance;
        struct testStruct
        {
            public int x;
            public int y;
            public bool affe;
        }

        [SetUp()]
        public void Setup()
        {
            ktdInstance = new KTD();
            ktdInstance.SINFONIServices = new ServiceRegistry();

            i32 = ktdInstance.GetKtdType("i32");
            ktd_string = ktdInstance.GetKtdType("string");

            intStruct = new KtdStruct("intStruct");
            intStruct.members["x"] = i32;
            intStruct.members["y"] = i32;

            serviceFunction = new ServiceFunctionDescription("function", new KtdType("void"));
            serviceFunction.Parameters.Add("intParameter", i32);
            serviceFunction.Parameters.Add("stringParameter", ktd_string);

            service = new SINFONIService("service");
            service.serviceFunctions.Add("function", serviceFunction);

            ktdInstance.SINFONIServices.services.Add("service", service);
            connection = new TestConnection();
            connection.Ktd = ktdInstance;
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
            KtdArray parameterArray = new KtdArray();
            parameterArray.elementType = i32;

            var serviceFunction = new ServiceFunctionDescription("arrayFunction", new KtdType("void"));
            serviceFunction.Parameters.Add("arrayParam", parameterArray);
            service.serviceFunctions.Add("arrayFunction", serviceFunction);
            var clientFunction = connection.GenerateClientFunction("service", "arrayFunction");
            Assert.DoesNotThrow(() => clientFunction(new int[] {1, 2, 3, 4}));
        }

        [Test()]
        public void CallShouldBeValidForMapParameters()
        {
            KtdMap parameterMap = new KtdMap();
            parameterMap.elementType = i32;
            parameterMap.keyType = ktd_string;

            var serviceFunction = new ServiceFunctionDescription("mapFunction", new KtdType("void"));
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
            var serviceFunction = new ServiceFunctionDescription("structFunction", new KtdType("void"));
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
