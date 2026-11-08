namespace Common.Packets.Game;

public class BoardPlayedPacket : AbstractPacket
{
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.BoardPlayed;
}