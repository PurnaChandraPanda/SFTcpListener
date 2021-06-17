# SFTcpListener
App explains setting up TcpListener in SF.

## About project
- SF Stateless TCP listener server project - `Listener1`
- Non-SF TCP client project - `Client1`
- Both the projects are in .NET Core.

### Define ICommunicationListener
- It is in `TcpCommunicationListener`, i.e. of type ICommunicationListener. 
- `OpenAsync` would start the `TcpListener` object.
- `CloseAsync` and `Abort` would stop the `TcpListener` object.
- Return the TcpListener object, which will be consumed on client side.

```
        public TcpListener GetTcpListener()
        {
            return _tcpListener;
        }
```

### Define StatelessService
- It is in `Listener1`.
- Initialize the instance listener with `TCPCommunicationListener` object.

```
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
```

- Update `RunAsync` to handle the server side operation, where it would read client payload/ process/ respond to the client.
- Prepare Tcp Server side operation in while loop as described in [TcpLister doc](https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=net-5.0).

### Define TcpClient
- Prepare the TcpClient operation as desribed in [TcpClient doc](https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=net-5.0).

```
                while (true)
                {
                    var message = Console.ReadLine();
                    Connect(message);                    
                }
```

## Simple data flow
![test data flow](https://github.com/PurnaChandraPanda/SFTcpListener/blob/main/images/data-flow.PNG)
