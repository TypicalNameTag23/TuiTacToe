namespace Common.Packets.Handshake;

public abstract class BaseAuthenticationPacket : AbstractRespondablePacket
{
    public string Username { get; private set; }
    public string Password { get; private set; }

    internal BaseAuthenticationPacket(PacketType packetType) : base(packetType) { }
    
    public BaseAuthenticationPacket(string username, string password, PacketType packetType) : base(packetType)
    {
        if (string.IsNullOrEmpty(username))
        {
            throw new ArgumentException("Username and password must neither be null nor empty.");
        }
        
        if (username.Length > short.MaxValue)
        {
            throw new ArgumentException("Username is too long.", nameof(username));
        }

        // if (passwordHash.Length != 64)
        // {
            // throw new ArgumentException("Password hash length must be 64 bytes.");
        // }
        
        Username = username;
        Password = password;
    }

    protected override void RegisterValues()
    {
        base.RegisterValues();
        
        RegisterDynamicSizeValue(nameof(Username), () => Username, value => Username = value);
        RegisterDynamicSizeValue(nameof(Password), () => Password, value => Password = value);//, 64);
    }
}