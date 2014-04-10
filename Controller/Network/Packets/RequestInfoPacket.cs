namespace Controller.Network.Packets
{
    public class RequestInfoPacket : Packet
    {
        public RequestInfoPacket() : base(0x01) { }
    }
}
