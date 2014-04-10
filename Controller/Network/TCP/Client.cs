using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Controller.Network.Packets;

namespace Controller.Network.TCP
{
    public partial class Client
    {
        private readonly Socket _socket;

        public event EventHandler<PacketEventArgs> PacketReceived = delegate { };
        public event EventHandler<PacketEventArgs> PacketSent = delegate { };
        public event EventHandler<ClientEventArgs> ClientDisconnected = delegate { };

        public IntPtr ClientHandle
        {
            get { return _socket.Handle; }
        }

        public EndPoint RemoteEndPoint
        {
            get { return _socket.RemoteEndPoint; }
        }

        public EndPoint LocalEndPoint
        {
            get { return _socket.LocalEndPoint; }
        }

        public Client()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        internal Client(Socket socket)
        {
            _socket = socket;
        }

        public bool Connect(EndPoint endPoint)
        {
            try
            {
                _socket.Connect(endPoint);
                return _socket.Connected; //not sure if needed?
            }
            catch
            {
                return false;
            }
        }

        public Packet ReceivePacket()
        {
            var id = ReadByte();
            if (id == 0xFF)
                return null;

            var size = ReadInt();
            if (size == -1)
                return null;

            var packet = PacketHelper.ConstructPacket(this, id, (uint)size);
            PacketReceived(this, new PacketEventArgs(this, packet));
            return packet;
        }

        /// <summary>
        /// Only the Id is read asynchronously!!
        /// </summary>
        public async Task<Packet> ReceivePacketAsync()
        {
            var id = await ReadByteAsync();
            var size = ReadInt();

            var packet = PacketHelper.ConstructPacket(this, id, (uint) size);
            PacketReceived(this, new PacketEventArgs(this, packet));
            return packet;
        }

        public void SendPacket(Packet packet)
        {
            if (packet == null) return;
            WriteByte(packet.Id);
            WriteInt((int)packet.Size);

            packet.WritePacket(this);

            //PacketSent(this, new PacketEventArgs(this, packet));
        }

        /// <summary>
        /// Only the Id is written asynchronously!!
        /// </summary>
        public async Task SendPacketAsync(Packet packet)
        {
            await WriteByteAsync(packet.Id);
            WriteInt((int) packet.Size);

            packet.WritePacket(this);

            //PacketSent(this, new PacketEventArgs(this, packet));
        }

        public void Disconnect()
        {
            try
            {
                if (_socket != null)
                {
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
                }
            }
            catch
            {
            }

            ClientDisconnected(this, new ClientEventArgs(this));
        }
    }
}
