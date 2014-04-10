using System;
using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public class PacketEventArgs : EventArgs
    {
        public Client Client { get; private set; }
        public Packet Packet { get; private set; }

        public PacketEventArgs(Client client, Packet packet)
        {
            Client = client;
            Packet = packet;
        }
    }
}
