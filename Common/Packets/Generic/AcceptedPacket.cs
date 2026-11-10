namespace Common.Packets.Generic;

public class AcceptedPacket : AbstractResponsePacket
{
    internal AcceptedPacket() : base(PacketType.Accepted) { }
    
    public AcceptedPacket(uint id) : base(id, PacketType.Accepted) { }
}