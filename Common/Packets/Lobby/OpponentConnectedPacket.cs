namespace Common.Packets.Lobby;

public class OpponentConnectedPacket : AbstractPacket
{
    public string Username { get; private set; }

    internal OpponentConnectedPacket() : base(PacketType.OpponentConnected) { }
    
    public OpponentConnectedPacket(string username) : base(PacketType.OpponentConnected)
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