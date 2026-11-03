namespace Common.Packets.Generic;

public class AcceptedPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.Accepted;
}