namespace Common.Packets.Handshake;

public class LoginPacket : BaseAuthenticationPacket
{
    
    internal LoginPacket() : base(PacketType.Login) { }

    public LoginPacket(string username, string password) 
        : base(username, password, PacketType.Login) { }
}