namespace Common.Packets.Lobby;

public class LobbyJoinInformationPacket : AbstractResponsePacket
{
    public string OpponentUsername { get; private set; }

    internal LobbyJoinInformationPacket() : base(PacketType.LobbyJoinInformation) { }
    
    public LobbyJoinInformationPacket(string opponentUsername, uint id) 
        : base(id, PacketType.LobbyJoinInformation)
    {
        if (opponentUsername.Length > short.MaxValue)
        {
            throw new ArgumentException("Username is too long.", nameof(opponentUsername));
        }
        
        OpponentUsername = opponentUsername;
    }

    protected override void RegisterValues()
    {
        base.RegisterValues();
        
        RegisterDynamicSizeValue(nameof(OpponentUsername), () => OpponentUsername, value => OpponentUsername = value);
    }
}