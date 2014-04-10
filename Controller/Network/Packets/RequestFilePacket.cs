using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    class RequestFilePacket : Packet
    {
        public string Path { get; private set; }

        public RequestFilePacket() : base(0x22) { }
        public RequestFilePacket(string path) : this()
        {
            Path = path;
        }

        public override void WritePacket(Client client)
        {
            client.WriteString(Path);
        }
    }
}
