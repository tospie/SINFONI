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

namespace SINFONI
{
    /// <summary>
    /// SINFONI Server provides the high level server object in which scope services are started. A SINFONI server can
    /// consist of several services each running their own transport and protocol and which thus can be queried
    /// individually. They all share the IDL that is provided by the SINFONI server and may implement all services or
    /// just a subset of services that are defined in the IDL
    /// </summary>
    public class SINFONIServer
    {
        /// <summary>
        /// Starts a new SINFONI server. This server provides its configuration document under the address that is
        /// provided in the constructor. When starting the server, the IDL given under the specified URL is parsed.
        /// Applications that access the server on its listening port will receive the config document that contains
        /// the implemented services as well as the IDL that is used by the server
        /// </summary>
        /// <param name="host">Host URL under which the server will be accessible</param>
        /// <param name="port">Specific port on which server is listening for incoming connections</param>
        /// <param name="path">Sub-Path on the server where the listener is running</param>
        /// <param name="idlURI">URI from where the IDL can be accessed</param>
        public SINFONIServer(string host, int port, string path, string idlURI)
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
            ServerConfigDocument.servers = new List<ServiceDescription>();

            ConfigURI = "http://" + host + ":" + port + path;
            if(!idlURI.Contains("http://"))
            {

                ServerConfigDocument.idlURL = ConfigURI + idlURI + "/";
                IdlPath = path + idlURI + "/";
            }
            startHttpListener();
        }

        /// <summary>
        /// Starts a new service under a given host, port and sub-path, using a transport and protocol that were previously
        /// Registered under a certain name
        /// </summary>
        /// <param name="host">Host where the service is running</param>
        /// <param name="port">Port where the service is running</param>
        /// <param name="path">Sub-Path on the host where the service is accessed</param>
        /// <param name="transportName">Name of the transport that should be used for communication with this service</param>
        /// <param name="protocolName">Name of the protocol that should be used for communication with this service</param>
        /// <returns></returns>
        public ServiceImplementation StartService(string host, int port, string path, string transportName, string protocolName)
        {
            ServiceImplementation service = new ServiceImplementation(Context.DefaultContext);
            ServiceDescription serviceServer =
                Context.DefaultContext.StartServer(host, port, transportName, protocolName, ServerConfigDocument, service.HandleNewClient);
            ServerConfigDocument.servers.Add(serviceServer);
            return service;
        }

        /// <summary>
        /// Shuts down the SINFONI Server and clears all registered services.
        /// </summary>
        public void ShutDown() {
            Listener.Stop();
            //TODO: We need to iterate over all services and check for transport connections that are still open and need to be closed!!

        }

        /// <summary>
        /// Adds new content to the IDL of the server during run time. Parses the content of the IDL immediately to have it
        /// available in SinTD and Service Descriptions, and appends the new content to the IDL document that is transmitted to
        /// the clients.
        /// </summary>
        /// <param name="idlAmendment">The part which should be added to the existing IDL</param>
        public void AmendIDL(string idlAmendment)
        {
            try
            {
                IdlContent += idlAmendment;
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not amend IDL by additional contents, reason : " + e.Message);
            }

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
            while (Listener.IsListening)
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
