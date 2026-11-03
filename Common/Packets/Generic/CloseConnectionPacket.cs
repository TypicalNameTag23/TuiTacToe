namespace Common.Packets.Generic;

public class CloseConnectionPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.CloseConnection;
}