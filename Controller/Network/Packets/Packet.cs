using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    public abstract class Packet
    {
        public byte Id { get; private set; }

        public static uint HeaderSize
        {
            get { return sizeof (byte) + sizeof(int); }
        }

        public uint Size { get; set; }

        protected Packet(byte id)
        {
            Id = id;
        }

        public virtual void WritePacket(Client client) { }
        public virtual void ReadPacket(Client client) { }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
