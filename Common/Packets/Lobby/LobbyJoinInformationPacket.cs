namespace Common.Packets.Lobby;

public class LobbyJoinInformationPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.LobbyJoinInformation;
}