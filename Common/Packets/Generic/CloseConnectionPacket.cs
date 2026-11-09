namespace Common.Packets.Generic;

public class CloseConnectionPacket : AbstractPacket
{
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    
    public override PacketType PacketType => PacketType.CloseConnection;
}