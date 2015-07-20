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
using System.Net;
using System.Web.Script.Serialization;
using SINFONI;

namespace JsonRpcProtocol
{
    public class JsonRPC : IProtocol
    {
        public string MimeType
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return "jsonrpc"; }
        }

        public object SerializeMessage(IMessage message)
        {
            string jsonSerializedMessage = "";
            CallObject callMessage = new CallObject();
            callMessage.id = message.ID;

            if (message.Type == MessageType.REQUEST)
            {
                callMessage.parameters = message.Parameters.ToArray();
                callMessage.method = message.MethodName;
            }

            else
            {
                callMessage.result = message.Result;
                callMessage.method = null;
            }

            jsonSerializedMessage = JsonSerializer.Serialize(callMessage);
            return jsonSerializedMessage;
        }

        public IMessage DeserializeMessage(object message)
        {
            string receivedMessage = message as string;
            // Small hack
            receivedMessage = receivedMessage.Replace("params", "parameters");

            CallObject receivedCall = JsonSerializer.Deserialize<CallObject>(receivedMessage);
            MessageBase deserializedMessage = new MessageBase();
            deserializedMessage.ID = receivedCall.id;
            if(receivedCall.parameters != null)
                deserializedMessage.Parameters = new List<object> (receivedCall.parameters);
            if (receivedCall.result != null)
                deserializedMessage.Result = receivedCall.result;
            deserializedMessage.MethodName = receivedCall.method;
            deserializedMessage.Type = GetMessageType(receivedCall);
            return deserializedMessage;
        }

        private MessageType GetMessageType(CallObject callMessage)
        {
            if (callMessage.method == null)
            {
                if(callMessage.error != null)
                {
                    return MessageType.EXCEPTION;
                }
                else
                {
                    return MessageType.RESPONSE;
                }
            }

            else
            {
                return MessageType.REQUEST;
            }
        }

        private JavaScriptSerializer JsonSerializer = new JavaScriptSerializer();
    }
}
