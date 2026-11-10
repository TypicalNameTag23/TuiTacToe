namespace Common.Packets.Lobby;

public class JoinLobbyPacket : AbstractRespondablePacket
{
    public int LobbyId { get; set; }

    internal JoinLobbyPacket() : base(PacketType.JoinLobby) { }
    
    public JoinLobbyPacket(int lobbyId) : base(PacketType.JoinLobby)
    {
        LobbyId = lobbyId;
    }

    protected override void RegisterValues()
    {
        base.RegisterValues();
        
        RegisterFixedSizeValue(() => LobbyId, value => LobbyId = value, sizeof(int));
    }
}