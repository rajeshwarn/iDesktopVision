using System.Drawing;
using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public class RequestScreenshotBlockPacket : Packet
    {
        public Point Block { get; private set; }

        public uint BlockHash { get; private set; }
        public uint Quality { get; private set; }

        public RequestScreenshotBlockPacket() : base(0x12)
        {
            Size = 16;
        }
        
        public RequestScreenshotBlockPacket(Point block, uint blockHash, uint quality) : this()
        {
            Block = block;
            BlockHash = blockHash;
            Quality = quality;
        }

        public override void WritePacket(Client client)
        {
            client.WritePoint(Block);
            client.WriteInt((int)BlockHash);
            client.WriteInt((int)Quality);

        }
    }
}
