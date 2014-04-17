using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DelegateArgumentMappingTests
{
    class TypeMappingTest
    {
        class ktdType
        {
            public string Name { get; private set; }

            public ktdType GetMember(string name)
            {
                return members[name];
            }

            Dictionary<string, ktdType> members;
        }

        class ktdMember
        {
            string Name { get;  set; }
            ktdType type;
        }


        void createKtdEntry()
        { }

        Dictionary<string, Type> ktd = new Dictionary<string, Type>();
    }    
}
