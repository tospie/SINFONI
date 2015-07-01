using System;
using System.Net;

namespace SINFONI
{
    public interface IWebClient
    {
        string DownloadString(string address);
    };

    public class WebClientWrapper : WebClient, IWebClient { };
}

