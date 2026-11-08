namespace Common.Packets.Lobby;

public class CreateLobbyPacket : AbstractPacket
{
    internal CreateLobbyPacket() { }
    
    public CreateLobbyPacket(ulong packetId) : base(packetId) { }
    
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.CreateLobby;
}