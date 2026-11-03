namespace Common.Packets.Game;

public class UpdateBoardPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.UpdateBoard;
}