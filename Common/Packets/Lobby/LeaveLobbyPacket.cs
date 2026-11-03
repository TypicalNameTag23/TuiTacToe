namespace Common.Packets.Lobby;

public class LeaveLobbyPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.LeaveLobby;
}