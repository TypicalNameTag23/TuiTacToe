namespace Common.Packets.Game;

public class UpdateBoardPacket : AbstractPacket
{
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.UpdateBoard;
}