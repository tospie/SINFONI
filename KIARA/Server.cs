using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Web.Script.Serialization;

namespace KIARA
{
    public class KIARAServer
    {
        public KIARAServer(string host, int port, string path, string idlURI)
        {
            ConfigHost = host;
            ConfigPort = port;
            ConfigPath = path;

            WebClient webClient = new WebClient();
            IdlContent = webClient.DownloadString(idlURI);

            IDLParser.Instance.ParseIDL(IdlContent);
            ServerConfigDocument = new Config();
            ServerConfigDocument.info = "TODO";
            ServerConfigDocument.idlURL = idlURI;
            ServerConfigDocument.servers = new List<Server>();

            ConfigURI = "http://" + host + ":" + port + path;
            if(!idlURI.Contains("http://"))
            {

                ServerConfigDocument.idlURL = ConfigURI + idlURI + "/";
                IdlPath = path + idlURI + "/";
            }
            startHttpListener();
        }

        public IServiceImpl StartService(string host, int port, string path, string transportName, string protocolName)
        {
            ServiceImpl service = new ServiceImpl(Context.DefaultContext);
            Server serviceServer =
                Context.DefaultContext.StartServer(host, port, transportName, protocolName, service.HandleNewClient);
            ServerConfigDocument.servers.Add(serviceServer);
            return service;
        }

        private void startHttpListener()
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add(ConfigURI);
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
            if (Listener.IsListening)
            {
                HttpListenerContext listenerContext = Listener.EndGetContext(result);
                listenerContext.Response.StatusCode = 200;
                listenerContext.Response.StatusDescription = "OK";
                Stream output = listenerContext.Response.OutputStream;
                byte[] buffer;
                if (listenerContext.Request.RawUrl == IdlPath)
                {
                    buffer = Encoding.UTF8.GetBytes(IdlContent);
                }
                else
                {
                    string configAsString = JsonSerializer.Serialize(ServerConfigDocument);
                    buffer = Encoding.UTF8.GetBytes(configAsString);
                }
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }

        private void CreateConfig()
        {
        }

        private string ConfigHost;
        private int ConfigPort;
        private string ConfigPath;

        private string IdlPath;
        private string IdlContent;

        private string ConfigURI;

        private Config ServerConfigDocument;

        HttpListener Listener;

        private JavaScriptSerializer JsonSerializer = new JavaScriptSerializer();

        Thread ListenerThread;
    }
}
