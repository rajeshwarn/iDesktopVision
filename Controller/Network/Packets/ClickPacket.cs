using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public class ClickPacket : Packet
    {
        public Point Point { get; private set; }
        public MouseButtons Buttons { get; private set; }
        public PressEvent PressEvent { get; private set; }

        public ClickPacket(Point point, MouseButtons buttons, PressEvent pressEvent) : base(0x14)
        {
            Point = point;
            Buttons = buttons;
            PressEvent = pressEvent;
        }

        public override void WritePacket(Client client)
        {
            client.WritePoint(Point);
            client.WriteInt((int)Buttons);
            client.WriteByte((byte)PressEvent);
        }
    }
}
