namespace Common.Packets.Lobby;

public class LobbyCreationInformationPacket : AbstractResponsePacket
{
    public int LobbyId { get; set; }

    internal LobbyCreationInformationPacket() : base(PacketType.LobbyCreationInformation) { }
    
    public LobbyCreationInformationPacket(int lobbyId, uint id) 
        : base(id, PacketType.LobbyCreationInformation)
    {
        LobbyId = lobbyId;
    }
    
    protected override void RegisterValues()
    {
        base.RegisterValues();
        
        RegisterDynamicSizeValue(() => LobbyId, value => LobbyId = value);
    }
}