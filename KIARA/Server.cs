using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        }

        private void startListener()
        {
            Listener = new HttpListener();
            Listener.Prefixes.Add(ConfigURI);
            Listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;

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
        }

        public IServiceImpl StartService(string host, int port, string path, string transportName, string protocolName)
        {
            ServiceImpl service = new ServiceImpl(Context.DefaultContext);
            Context.DefaultContext.StartServer(host, port, transportName, protocolName, service.HandleNewClient);
            return service;
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
