using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using Controller.Network.TCP;
using LZ4;

namespace Controller.Network.Packets
{
    public class SendScreenshotBlockPacket : Packet
    {

        public Point Block { get; private set; }
        public Bitmap Image { get; private set; }
        public uint BlockHash { get; private set; }
        public bool Compressed { get; private set; }

        public SendScreenshotBlockPacket() : base (0x13)
        {
        }

        public override void ReadPacket(Client client)
        {
            Block = client.ReadPoint();
            var bytes = client.ReadByteArray();
            BlockHash = (uint)client.ReadInt();
            Compressed = client.ReadByte() != 0x00;
            var size = client.ReadInt();

            if (bytes == null) return;

            if (Compressed)
            {
                try
                {
                    var buffer = LZ4Codec.Decode(bytes, 0, bytes.Length, size);
                    bytes = buffer;
                }
                catch (ArgumentException arg)
                {
                    Console.WriteLine(arg);
                }
                
            }

            if (bytes.Length == 0) return;
            using (var ms = new MemoryStream(bytes))
                Image = (Bitmap)Bitmap.FromStream(ms);
        }
    }
}
