using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public class SendInfoPacket : Packet
    {
        public string ComputerName { get; set; }
        public string UserName { get; set; }
        public string OS { get; set; }
        //public byte[][] FileHashes { get; set; }

        public SendInfoPacket() : base(0x02)
        {
        }

        public override void ReadPacket(Client client)
        {
            ComputerName = client.ReadString();
            UserName = client.ReadString();
            OS = client.ReadString();
            //FileHashes = new[]
            //{
            //    Client.ReadByteArray(),
            //    Client.ReadByteArray()
            //};
        }
    }
}
