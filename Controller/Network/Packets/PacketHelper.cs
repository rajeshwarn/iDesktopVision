using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Controller.Network.TCP;

namespace Controller.Network.Packets
{
    internal static class PacketHelper
    {
        private readonly static Dictionary<byte, Type> _packetTypes; 

        static PacketHelper()
        {
            _packetTypes = new Dictionary<byte, Type>();
            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Union(Assembly.GetEntryAssembly().GetTypes()
                .Union(Assembly.GetCallingAssembly().GetTypes()))
                .Where(type => typeof (Packet).IsAssignableFrom(type) && type != typeof(Packet));

            foreach (var type in types)
            {
                try
                {
                    var id = ((Packet) Activator.CreateInstance(type)).Id;
                    _packetTypes[id] = type;
                }
                catch
                {
                    Console.WriteLine(type);
                }
            }
        }

        public static Packet ConstructPacket(Client client, byte id, uint size)
        {
            Type type;
            if (_packetTypes.TryGetValue(id, out type) == false)
                return null;

            var packet = (Packet) Activator.CreateInstance(type);
            packet.Size = size;
            packet.ReadPacket(client);
            return packet;
        }
    }
}