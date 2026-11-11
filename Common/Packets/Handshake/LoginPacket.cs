namespace Common.Packets.Handshake;

public class LoginPacket : BaseAuthenticationPacket
{
    
    internal LoginPacket() : base(PacketType.Login) { }

    public LoginPacket(string username, byte[] passwordHash) 
        : base(username, passwordHash, PacketType.Login) { }
}