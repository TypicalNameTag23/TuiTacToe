namespace Common.Packets.Lobby;

public class LeaveLobbyPacket : AbstractPacket
{
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.LeaveLobby;
}