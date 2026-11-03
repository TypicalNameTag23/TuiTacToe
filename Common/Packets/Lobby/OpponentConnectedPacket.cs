namespace Common.Packets.Lobby;

public class OpponentConnectedPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.OpponentConnected;
}