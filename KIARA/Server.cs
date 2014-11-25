using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace KIARA
{
    public class KIARAServer
    {
        public KIARAServer(string host, int port, string path, string idlURI)
        {
            ConfigHost = host;
            ConfigPort = port;
            ConfigPath = path;

            IDLParser.Instance.ParseIDLFromUri(idlURI);

            ConfigURI = "http://" + host + ":" + port + "/" + path;
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

        private string ConfigURI;

        HttpListener Listener;

        Thread ListenerThread;
    }
}
