using Common.Packets.Handshake;

namespace Common.Packets.Generic;

public class AcceptedPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.Accepted;

    public AcceptedPacket() { }
    
    public AcceptedPacket(ulong id) : base(id) { }
}