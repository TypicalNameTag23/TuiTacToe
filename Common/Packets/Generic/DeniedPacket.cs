namespace Common.Packets.Generic;

public class DeniedPacket : AbstractResponsePacket
{
    internal DeniedPacket() : base(PacketType.Denied) { }
    
    public DeniedPacket(uint id) : base(id, PacketType.Denied) { }
}