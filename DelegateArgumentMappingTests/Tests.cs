using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DelegateArgumentMappingTests
{
    public class Tests
    {
        public Tests()
        {
            testStuff();
            testGetMembers();
            testIsMappable();
        }

        delegate object DelegateFunc(object[] args);

        // THIS would be the
        // LOCAL NATIVE FUNCTION
        private int test(int first, float second)
        {
            return 0;
        }

        // THIS would be the
        // *FUNCTION ACCORDING TO IDL*
        private int test2(float second, int first)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object> {
                {"first", first},
                {"second", second}
            };

            Delegate testFunc = (Func<int, float, int>)test;
            var currentMethod = MethodInfo.GetCurrentMethod();
            var delegateMethod = testFunc.Method;
            var parameters = delegateMethod.GetParameters();
            var thing = parameters.Select(p => dictionary[p.Name]);
            var arguments = thing.ToArray();

            testFunc.Method.Invoke(this, arguments);
            return 0;
        }

        struct vecStruct
        {
            public float x;
            public float y;
            public float z;
        };
    
        class vecMembers
        {
            public float x;
            public float y;
            public float z;
        }

        class vecProperties
        {
            public vecProperties()
            {
                x = 1;
            }
    
            public float x { get; set; }
            public float y { get; set; }
            public float z { get; set; }
        }
    
        void testGetMembers()
        {
            var structObj = new vecStruct{x = 1, y = 2, z = 3};
            var vecObj = new vecMembers{ x = 1, y = 2, z = 3 };
            var vecObjProp = new vecProperties {y = 2, z = 3 };

            var baseType = 2.0f;

            var baseFields = baseType.GetType().GetFields();
            var baseProps = baseType.GetType().GetProperties();

            var structObjMembers = structObj.GetType().GetFields();
            var vecObjMembers = vecObj.GetType().GetFields();
            var vecPropMembers = vecObjProp.GetType().GetProperties();
            var p = vecObjMembers.First();
            var b = p.GetValue(vecObj);
        }
    
        void testIsMappable()
        {
            KtdType int16 = new KtdType();
            int16.name = "i16";
            short value = 42;
            var can = int16.canBeAssignedFromType(value.GetType());
        }
    
        void testStuff()
        {
            DelegateFunc testDelegate = (DelegateFunc)delegate(object[] a)
            {
                return 0;
            };


            Delegate testFunc = (Func<int, float, int>)test;
            Type testFuncSignature = typeof(Func<int, float, int>);

            // THIS would be the
            // *MAPPING FUNCTION*
            // That is executed when the respective service was called from the network.
            // In our example, args would be a list { 0: 2.0, 1: 1}
            var mapThenCallHandler = (DelegateFunc)delegate(object[] args)
            {

                // THIS is how the order of parameters is defined in the ID
                // (will be CACHED after IDL processing) and might come from an idl that
                // contained an entry like
                //  int myFunction(float second, int first); 
                List<string> idlDefinition = new List<string> { "second", "first" };

                // THIS contains the argument values assigned to type names as
                // specified in the IDL. The keys of the dictionary are determined by
                // the IDL, whereas the Values come from the object[] passed as args
                Dictionary<string, object> dictionary = new Dictionary<string, object>();

                // THIS assigns the argument values to the parameter names
                // In our example, it will result in a dictionary with the entries
                // { "second" : 2.0, "first" : 1 }
                int idx = 0;
                foreach (var a in args)
                {
                    dictionary[idlDefinition[idx++]] = a;
                }

                // THIS retrieves names and orders of Parameters from the target
                // native function
                // (Can be CACHED when registering the native function as handler for the
                // remote function). In our example, pars would be a ParameterInfo-Array with
                // with values:
                // { 0: [System.Int32, 'first'], 1: [System.Single, 'second'] }
                var delegateMethod = testFunc.Method;
                var pars = delegateMethod.GetParameters();

                // THIS maps the parameters of our target function to the values stored in our
                // dictionary for the respective name. I.e., each parameter in the target method's
                // parameters will be mapped to the value stored in the dictionary for the respective name
                // (Which is actually exactly what we want to achieve)
                var mappedValues = pars.Select(p => dictionary[p.Name]);

                // THIS creates a parameter Array in the correct order for the target
                // Native function according to the previous mapping
                // i.e. in our example {1, 2.0}
                var arguments = mappedValues.ToArray();

                // THIS invokes the target native function (NO dynamic invoke!)
                var result = testFunc.Method.Invoke(this, arguments);

                return result;
            };
            
// Create Parameter set mockup. This is how the message would arrive from the network,
// and how order of parameters would be defined in the IDL
object[] parameters = new object[2];
parameters[0] = 2.0f;
parameters[1] = 1;

// mapThenCallHandler can directly be called as the signature is always the same.
var r = mapThenCallHandler(parameters);

            var type = testFunc.Method.ReturnType;
            var parameterInfo = testFunc.Method.GetParameters();
            
            Console.WriteLine("Type: {0}", testFuncSignature.ToString());
            Console.Write("Parameters: ");
            foreach (var info in parameterInfo)
            {
                Console.Write("{0}, ", info);
            }
            Console.WriteLine();           

        }
    }
}
