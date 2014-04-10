using System.Collections.Generic;
using System.Linq;
using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public class SendPathContentPacket : Packet
    {
        public string Path { get; private set; }
        public Dictionary<string, long> PathListing { get; private set; }

        public SendPathContentPacket() : base(0x21) { }

        public override void ReadPacket(Client client)
        {
            Path = client.ReadString();
            PathListing = client.ReadDictionaryStringLong();
        }
    }
}
