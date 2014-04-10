using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public class RequestPathContentPacket : Packet
    {
        public string Path { get; set; }

        public RequestPathContentPacket() : base(0x20)
        {
        }

        public RequestPathContentPacket(string path) : this()

        {
            Path = path;
            Size = (uint) (Path.Length + sizeof (int));
        }

        public override void WritePacket(Client client)
        {
            client.WriteString(Path);
        }
    }
}
