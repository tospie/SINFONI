using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KIARA;

namespace WSJProtocol
{
    public class WSJConnection : Connection
    {

        protected IClientFunctionCall CallFunc(string funcName, params object[] args)
        {
            int callID = getValidCallID();

            // Register delegates as callbacks. Pass their registered names instead.
            List<int> callbacks;
            List<object> convertedArgs = convertCallbackArguments(args, out callbacks);
            // List<object> callMessage = createCallMessage(callID, funcName, callbacks, convertedArgs);

            IClientFunctionCall callObj = null;
            /*
            if (!IsOneWay(funcName))
            {
                callObj = wsjFuncCallFactory.Construct();

                // It is important to add an active call to the list before sending it, otherwise we may end up
                // receiving call-reply before this happens, which will trigger unnecessary call-error and crash the
                // other end.
                lock (activeCalls)
                    activeCalls.Add(callID, callObj);
            }

            SendMessage(callMessage);
            */

            return callObj;
        }

        private List<object> convertCallbackArguments(object[] args, out List<int> callbacks)
        {
            callbacks = new List<int>();
            return new List<object>();
        }

        private int getValidCallID()
        {
            return -1;
        }
    }
}
