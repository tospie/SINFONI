﻿// This file is part of SINFONI.
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

using SINFONI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace FiVESJson
{
    public class FiVESJsonProtocol : IProtocol
    {
        public string MimeType
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return "fives-json"; }
        }

        public object SerializeMessage(IMessage message)
        {
            List<object> messageObject = new List<object>();
            messageObject.Add(messageTypeAsString(message.Type));
            messageObject.Add(message.ID);
            if (message.Type == MessageType.REQUEST)
            {
                messageObject.Add(message.MethodName);
                messageObject.Add(new List<int>()); // this would have been callbacks; not supported like this anymore at this point
                messageObject.AddRange(message.Parameters);
            }
            else if (message.Type == MessageType.RESPONSE)
            {
                messageObject.Add(!message.IsException); // position 2 encodes SUCCESS of message for response
                messageObject.Add(message.Result); // Result or exception message, depending on whether message is success or not
            }
            else if (message.Type == MessageType.EXCEPTION)
            {
                messageObject.Add(message.Result); // Reason of Exception encoded in result field.
            }

            string serializedMessage = JsonSerializer.Serialize(messageObject);
            return serializedMessage;
        }

        public IMessage DeserializeMessage(object message)
        {
            MessageBase deserializedMessage = new MessageBase();
            var data = JsonSerializer.Deserialize<List<object>>(message as string);

            deserializedMessage.Type = getMessageType(data[0] as string);
            deserializedMessage.IsException = deserializedMessage.Type == MessageType.EXCEPTION;
            deserializedMessage.ID = (int)data[1];

            if (deserializedMessage.Type == MessageType.REQUEST)
            {
                deserializedMessage.MethodName = data[2] as string;
                deserializedMessage.Parameters = data.GetRange(4, data.Count - 4);
            }

            if (deserializedMessage.Type == MessageType.RESPONSE)
            {
                deserializedMessage.IsException = !((bool)data[2]); // Position 2 of message encodes SUCCESS
                deserializedMessage.Result = data[3];
            }

            else if (deserializedMessage.Type == MessageType.EXCEPTION)
            {
                deserializedMessage.Result = data[2] as string;
            }
            return deserializedMessage;
        }

        private string messageTypeAsString(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.REQUEST: return "call";
                case MessageType.RESPONSE: return "call-reply";
                case MessageType.EXCEPTION: return "call-error";
                default: return "call-error";
            }
        }

        private MessageType getMessageType(string typeAsString)
        {
            if (typeAsString == "call")
                return MessageType.REQUEST;
            else if (typeAsString == "call-reply")
                return MessageType.RESPONSE;
            else if (typeAsString == "call-error")
                return MessageType.EXCEPTION;

            throw new InvalidMessageTypeException(typeAsString);
        }

        private JavaScriptSerializer JsonSerializer = new JavaScriptSerializer();
    }
}
