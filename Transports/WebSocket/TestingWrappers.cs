using System;
using KIARA;

namespace KIARA.Transport.WebSocketTransport
{
    public interface IWSJServerFactory
    {
        IWSServer Construct(Action<Connection> onNewClient);
    }

    public class WSJServerFactory : IWSJServerFactory
    {
        public IWSServer Construct(Action<Connection> onNewClient)
        {
            return new WSServer();
        }
    }

    public interface IWSJFuncCallFactory
    {
        FuncCallBase Construct(string serviceName, string methodName);
    }

    public class WSJFuncCallFactory : IWSJFuncCallFactory
    {
        public FuncCallBase Construct(string serviceName, string methodName)
        {
            return new FuncCallBase(serviceName, methodName);
        }
    }

    public interface IWebSocketFactory
    {
        ITransportConnection Construct(string uri);
    }

    public class WebSocketFactory : IWebSocketFactory
    {
        public ITransportConnection Construct(string uri)
        {
            return new WebSocketSocketAdapter(uri);
        }
    }
}

