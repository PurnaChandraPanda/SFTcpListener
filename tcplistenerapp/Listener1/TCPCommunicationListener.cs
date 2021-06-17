using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Listener1
{
    public class TCPCommunicationListener : ICommunicationListener
    {
        private readonly StatelessServiceContext _serviceContext;
        private readonly TcpListener _tcpListener;
        private readonly int _port;

        public TCPCommunicationListener(StatelessServiceContext serviceContext)
        {

            _serviceContext = serviceContext;

            //TCPListenerEndpoint matches the name of the listener in ServiceManiest.xml.
            _port = serviceContext.CodePackageActivationContext.GetEndpoint("TCPListenerEndpoint").Port;

            //Listen on any IP, but the specified port.
            _tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, _port));
        }

        public TcpListener GetTcpListener()
        {
            return _tcpListener;
        }

        public void Abort()
        {
            StopTcpListener();
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            StopTcpListener();
            return Task.FromResult(true);
        }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            ServiceEventSource.Current.ServiceMessage(_serviceContext, "Starting TCP listener on port {0}", _port);
            _tcpListener.Start();

            //generate published URI from IP/FQDN and port number.
            string uriPublished = $"tcp:{FabricRuntime.GetNodeContext().IPAddressOrFQDN}:{_port}";

            ServiceEventSource.Current.ServiceMessage(_serviceContext, "TCP listener started and published with address {0}", uriPublished);

            return Task.FromResult(uriPublished);
        }

        private void StopTcpListener()
        {
            ServiceEventSource.Current.ServiceMessage(_serviceContext, "Stopping TCP listener on port {0}", _port);

            _tcpListener.Stop();

            ServiceEventSource.Current.ServiceMessage(_serviceContext, "TCP listener stopped.");
        }
    }
}
