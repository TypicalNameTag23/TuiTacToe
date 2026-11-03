namespace Common.Packets.Lobby;

public class LobbyCreatePacket : AbstractPacket
{
    public LobbyCreatePacket() { }
    
    public LobbyCreatePacket(ulong packetId) : base(packetId) { }
    
    public override PacketType PacketType => PacketType.LobbyCreate;
}