using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using KIARA;

namespace HttpJSONProtocol
{
    class HttpJSONServer
    {
        public HttpJSONServer(Action<Connection> onClientConnected)
        {
            OnClientConnected = onClientConnected;
        }

        internal void StartServer(string hostName, int port)
        {
            HostName = hostName;
            Port = port;
            openListener();
        }

        private void openListener()
        {
            string hostPrefix = "http://+:" + Port + "/";
            Listener = new HttpListener();

            Listener.Prefixes.Add(hostPrefix);
            Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

            Listener.Start();
            ListenerThread = new Thread(new ParameterizedThreadStart(startListener));
            ListenerThread.Start();
        }

        private void startListener(object s)
        {
            while (true)
            {
                ProcessRequest();
            }
        }

        private void ProcessRequest()
        {
            var result = Listener.BeginGetContext(HandleRequest, Listener);
            result.AsyncWaitHandle.WaitOne();
        }

        private void HandleRequest(IAsyncResult result)
        {
            if (serverConnection == null)
            {
                serverConnection = new HttpJSONConnection();
                OnClientConnected(serverConnection);
            }

            var context = Listener.EndGetContext(result);
            var data_as_text = new StreamReader(context.Request.InputStream,
                context.Request.ContentEncoding)
                .ReadToEnd();

            CallObject callObject = (CallObject)JsonSerializer.Deserialize(data_as_text, typeof(CallObject));
            var functionResult = serverConnection.HandleRequest(callObject);
            Console.WriteLine("Received a request, sending Hello World");
            context.Response.StatusCode = 200;
            context.Response.StatusDescription = "OK";

            string responseString = JsonSerializer.Serialize(functionResult);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            context.Response.ContentLength64 = buffer.Length;
            System.IO.Stream output = context.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }

        private string HostName;
        private int Port;

        private static HttpListener Listener;
        private Thread ListenerThread;
        private JavaScriptSerializer JsonSerializer = new JavaScriptSerializer();
        Action<Connection> OnClientConnected;
        HttpJSONConnection serverConnection = null;
    }
}
