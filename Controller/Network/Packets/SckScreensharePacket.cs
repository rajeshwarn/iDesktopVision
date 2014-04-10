using System.Drawing;
using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public class SckScreensharePacket : Packet
    {
        public Size Blocks { get; set; }
        public uint Quality { get; set; }

        public SckScreensharePacket() : base(0x11)
        {
            Blocks = new Size(0, 0);
            Quality = 100;
        }

        public SckScreensharePacket(Size blocks, uint quality) : this()
        {
            Blocks = blocks;
            Quality = quality;
        }

        public override void WritePacket(Client client)
        {
            client.WriteSize(Blocks);
            client.WriteInt((int)Quality);
        }
    }
}
