using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA;
using NUnit.Framework;

namespace KIARAUnitTests
{
    [TestFixture()]
    class TestIDLParser
    {
        [Test()]
        public void testRegexParsing()
        {
            var parser = new IDLParser();
            parser.parseIDL();
        }
    }
}
