namespace Common.Packets.Lobby;

public class CreateLobbyPacket : AbstractPacket
{
    public CreateLobbyPacket() { }
    
    public CreateLobbyPacket(ulong packetId) : base(packetId) { }
    
    public override PacketType PacketType => PacketType.CreateLobby;
}