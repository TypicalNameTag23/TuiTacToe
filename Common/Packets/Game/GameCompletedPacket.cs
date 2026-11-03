namespace Common.Packets.Game;

public class GameCompletedPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.GameCompleted;
}