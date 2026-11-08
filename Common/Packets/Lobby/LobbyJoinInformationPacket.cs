namespace Common.Packets.Lobby;

public class LobbyJoinInformationPacket : AbstractPacket
{
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.LobbyJoinInformation;
}