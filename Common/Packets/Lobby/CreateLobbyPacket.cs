namespace Common.Packets.Lobby;

public class CreateLobbyPacket : AbstractPacket
{
    public CreateLobbyPacket() { }
    
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.CreateLobby;
}