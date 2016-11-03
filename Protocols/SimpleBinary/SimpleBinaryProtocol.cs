using SINFONI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SimpleBinary
{
    public class SimpleBinaryProtocol : IProtocol
    {
        public string MimeType
        {
            get { return "binary"; }
        }

        public string Name
        {
            get { return "simple-binary"; }
        }

        public object SerializeMessage(IMessage message)
        {
            byte[] messageAsBytestream;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, message);
                messageAsBytestream = ms.ToArray();
            }
            return messageAsBytestream;
        }

        public IMessage DeserializeMessage(object message)
        {
            byte[] messageBytes = (byte[])message;
            MemoryStream ms = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            ms.Write(messageBytes, 0, messageBytes.Length);
            ms.Seek(0, SeekOrigin.Begin);
            IMessage deserializedMessage = (IMessage)bf.Deserialize(ms);

            return deserializedMessage;
        }
    }
}
