namespace Common.Packets.Handshake;

public class RegisterPacket : BaseAuthenticationPacket
{
    public override PacketType PacketType => PacketType.Register;

    internal RegisterPacket() { }
    
    public RegisterPacket(string username, byte[] passwordHash) : base(username, passwordHash) { }
}