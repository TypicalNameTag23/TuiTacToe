namespace Common.Packets.Handshake;

public class OpenConnectionPacket : AbstractPacket
{
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.OpenConnection;
}