namespace Common.Packets.Lobby;

public class OpponentDisconnectedPacket : AbstractPacket
{
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.OpponentDisconnected;
}