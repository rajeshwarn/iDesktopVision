using System.Runtime.InteropServices;
using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public class SendFilePacket : Packet
    {
        public ulong Offset { get; private set; }
        public ulong Count { get; private set; }
        public byte[] Content { get; private set; }
        public string Path { get; private set; }

        public SendFilePacket() : base(0x23) { }

        public SendFilePacket(byte[] data, ulong offset, ulong count, string path) : this()
        {
            Content = data;
            Offset = offset;
            Count = count;
            Path = path;
        }

        public override void WritePacket(Client client)
        {
            client.WriteLong((long)Count);
            client.WriteLong((long)Offset);
            client.WriteByteArray(Content, 0, (int)Count); // NOTE: The buffer could be larger then the count of bytes!!
            client.WriteString(Path);
        }

        public override void ReadPacket(Client client)
        {
            Count = (ulong)client.ReadLong();
            Offset = (ulong)client.ReadLong();
            Content = client.ReadByteArray();
            Path = client.ReadString();
        }
    }
}
