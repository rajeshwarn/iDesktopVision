using System.IO;
using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public class  UpdateFilePacket : Packet
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; private set; }

        public UpdateFilePacket() : base(0xFF) { }
        public UpdateFilePacket(string path) : this() 
        {
            FileName = Path.GetFileName(path);
            FileBytes = File.ReadAllBytes(path);
        }

        public override void WritePacket(Client client)
        {
            client.WriteString(FileName);
            client.WriteByteArray(FileBytes);
        }
    }
}
