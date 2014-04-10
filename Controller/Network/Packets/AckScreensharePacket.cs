using System.Drawing;
using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public class AckScreensharePacket : Packet
    {
        public Size BlockSize { get; set; }
        public Size Blocks { get; set; }

        public AckScreensharePacket() : base(0x10)
        {
            Size = 4*sizeof (int); //Size.Width + Size.Height + Blocks
        }

        public AckScreensharePacket(Size blockSize, Size blocks) : this()
        {
            BlockSize = blockSize;
            Blocks = blocks;
        }

        public override void ReadPacket(Client client)
        {
            BlockSize = client.ReadSize();
            Blocks = client.ReadSize();
        }
    }
}
