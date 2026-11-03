namespace Common.Packets.Lobby;

public class OpponentDisconnectedPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.OpponentDisconnected;
}