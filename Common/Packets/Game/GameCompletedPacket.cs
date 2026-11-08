namespace Common.Packets.Game;

public class GameCompletedPacket : AbstractPacket
{
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.GameCompleted;
}