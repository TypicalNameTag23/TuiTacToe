namespace Common.Packets.Lobby;

public class LobbyJoinInformationPacket : AbstractResponsePacket
{
    public string Username { get; private set; }

    internal LobbyJoinInformationPacket() : base(PacketType.LobbyJoinInformation) { }
    
    public LobbyJoinInformationPacket(string username, uint id) 
        : base(id, PacketType.LobbyJoinInformation)
    {
        if (username.Length > short.MaxValue)
        {
            throw new ArgumentException("Username is too long.", nameof(username));
        }
        
        Username = username;
    }

    protected override void RegisterValues()
    {
        base.RegisterValues();
        
        RegisterDynamicSizeValue(() => Username, value => Username = value);
    }
}