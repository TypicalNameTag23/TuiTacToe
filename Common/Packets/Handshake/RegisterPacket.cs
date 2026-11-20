namespace Common.Packets.Handshake;

public class RegisterPacket : BaseAuthenticationPacket
{
    internal RegisterPacket() : base(PacketType.Register) { }
    
    public RegisterPacket(string username, string password) 
        : base(username, password, PacketType.Register) { }
}