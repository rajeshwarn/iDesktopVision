using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public class KeyboardPacket : Packet
    {
        public Keys Key { get; private set; }
        public PressEvent PressEvent { get; private set; }

        public KeyboardPacket(Keys key, PressEvent pressEvent) : base(0x15)
        {
            Key = key;
            PressEvent = pressEvent;
        }

        public override void WritePacket(Client client)
        {
            client.WriteByte((byte)Key);    // BYTE bVk (keydb_event) <= No need to serialize as int ;)
            client.WriteByte((byte)PressEvent);
        }
    }
}
