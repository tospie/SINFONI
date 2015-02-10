using KIARA;
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
            throw new NotImplementedException();
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

        private JavaScriptSerializer JsonSerializer = new JavaScriptSerializer();
    }
}
