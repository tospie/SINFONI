using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DelegateArgumentMappingTests
{
    class Connection : KIARAPlugin.Connection
    {
        

        protected override KIARAPlugin.IFuncCall CallFunc(string funcName, params object[] args)
        {
            throw new NotImplementedException();
        }

        public override event EventHandler Closed;

        public override void Disconnect()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessIDL(string parsedIDL)
        {
            throw new NotImplementedException();
        }

        protected override void RegisterHandler(string funcName, Delegate handler)
        {
            throw new NotImplementedException();
        }
    }
}
