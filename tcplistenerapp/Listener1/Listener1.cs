using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Listener1
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Listener1 : StatelessService
    {
        private TCPCommunicationListener _commListener;

        public Listener1(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            var instanceListener = new ServiceInstanceListener(serviceContext =>
            {
                _commListener = new TCPCommunicationListener(serviceContext);
                return _commListener;                
            });             

            return new ServiceInstanceListener[] {
                instanceListener
            };
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            while (true)
            {
                TcpServerOp();

                cancellationToken.ThrowIfCancellationRequested();
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        private void TcpServerOp()
        {
            String data = null;
            Byte[] bytes = new Byte[256];
            int i;

            // Get tcp listener
            var tcpListener = _commListener.GetTcpListener();
            
            // Accept tcp client
            var tcpClient = tcpListener.AcceptTcpClient();

            ServiceEventSource.Current.ServiceMessage(this.Context, "Connected!");
            
            // Get a stream object for reading and writing
            NetworkStream stream = tcpClient.GetStream();
            
            // Loop to receive all the data sent by the client.
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = Encoding.ASCII.GetString(bytes, 0, i);
                ServiceEventSource.Current.ServiceMessage(this.Context, "Received: {0}", data);

                // Process the data sent by the client.
                data = data.ToUpper();

                byte[] msg = Encoding.ASCII.GetBytes(data);

                // Send back a response.
                stream.Write(msg, 0, msg.Length);
                ServiceEventSource.Current.ServiceMessage(this.Context, "Sent: {0}", data);
            }

            // Shutdown and end connection
            tcpClient.Close();
        }
    }
}
