namespace Common.Packets.Lobby;

public class OpponentConnectedPacket : AbstractPacket
{
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.OpponentConnected;
}