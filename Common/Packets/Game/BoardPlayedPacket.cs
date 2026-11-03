namespace Common.Packets.Game;

public class BoardPlayedPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.BoardPlayed;
}