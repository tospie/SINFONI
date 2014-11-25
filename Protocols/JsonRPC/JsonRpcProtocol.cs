using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace KIARA.Protocols.JsonRPC
{
    public class JsonRpcProtocol : IProtocol
    {
        public string MimeType
        {
            get { throw new NotImplementedException(); }
        }

        public string Name
        {
            get { return "json-rpc"; }
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
            CallObject receivedCall = JsonSerializer.Deserialize<CallObject>(message as string);
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
