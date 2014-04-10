using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Controller.Network.TCP
{
    public class Server 
    {
        private readonly Pool<SocketAsyncEventArgs> _socketAsyncEventArgsPool;
        private readonly Socket _socket;

        public event EventHandler<ClientEventArgs> ClientConnected;
        public event EventHandler<ClientEventArgs> ClientDisconnected;

        public readonly List<Client> ClientList;

        public Server(int port)
        {
            _socketAsyncEventArgsPool = new Pool<SocketAsyncEventArgs>(() => new SocketAsyncEventArgs());
            ClientList = new List<Client>();
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));
            _socket.Listen(250);

            ClientConnected = delegate { };
            ClientDisconnected = (o, e) => ClientList.Remove(e.Client);
        }

        public void AcceptAsync()
        {
            var eventArgs = _socketAsyncEventArgsPool.GetObject();
            eventArgs.Completed += AcceptHandler;

            if (_socket.AcceptAsync(eventArgs) == false)
                AcceptHandler(this, eventArgs);
        }

        public void StartAccept()
        {
            ClientConnected += (sender, args) => AcceptAsync();
            AcceptAsync();
        }

        private void AcceptHandler(object sender, SocketAsyncEventArgs eventArgs)
        {
            var client = new Client(eventArgs.AcceptSocket);
            client.ClientDisconnected += ClientDisconnected;
            ClientList.Add(client);

            ClientConnected(this, new ClientEventArgs(client));
        }
    }
}
