namespace Common.Packets.Handshake;

public abstract class BaseAuthenticationPacket : AbstractRespondablePacket
{
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }

    internal BaseAuthenticationPacket(PacketType packetType) : base(packetType) { }
    
    public BaseAuthenticationPacket(string username, string passwordHash, PacketType packetType) : base(packetType)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("Username and password must neither be null nor empty.");
        }
        
        if (username.Length > short.MaxValue)
        {
            throw new ArgumentException("Username is too long.", nameof(username));
        }
        
        // todo: password hash validation
        
        Username = username;
        PasswordHash = passwordHash;
    }

    protected override void RegisterValues()
    {
        base.RegisterValues();
        
        RegisterDynamicSizeValue(() => Username, value => Username = value);
        RegisterFixedSizeValue(() => PasswordHash, value => PasswordHash = value, 64);
    }
}