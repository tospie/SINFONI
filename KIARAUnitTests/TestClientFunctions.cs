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
    class TestClientFunctions
    {
        KiaraService service;
        ServiceFunctionDescription serviceFunction;

        KtdType i32;
        KtdType ktd_string;

        [SetUp()]
        public void Setup()
        {
            KTD.Instance = new KTD();
            ServiceRegistry.Instance = new ServiceRegistry();

            i32 = KTD.Instance.GetKtdType("i32");
            ktd_string = KTD.Instance.GetKtdType("string");

            serviceFunction = new ServiceFunctionDescription("function", new KtdType("void"));
            serviceFunction.parameters.Add("intParameter", i32);
            serviceFunction.parameters.Add("stringParameter", ktd_string);

            service = new KiaraService("service");
            service.serviceFunctions.Add("function", serviceFunction);

            ServiceRegistry.Instance.services.Add("service", service);
        }
        
        [Test()]
        public void CallShouldBeValidForCorrectParameters()
        {            
            Connection connection = new Connection();

            var clientFunction = connection.GenerateClientFunction("service", "function");
            Assert.DoesNotThrow(() => clientFunction(1, "Hello World"));
        }

        [Test()]
        public void CallShouldThrowExceptionForWrongParameterCount()
        {
            Connection connection = new Connection();

            var clientFunction = connection.GenerateClientFunction("service", "function");
            Assert.Throws<ParameterMismatchException>(() => clientFunction("Hello World"));
        }

        [Test()]
        public void CallShouldThrowExceptionForWrongParameterType()
        {
            Connection connection = new Connection();

            var clientFunction = connection.GenerateClientFunction("service", "function");
            Assert.Throws<ParameterMismatchException>(() => clientFunction(1.0f, "Hello World"));
        }
    }
}
