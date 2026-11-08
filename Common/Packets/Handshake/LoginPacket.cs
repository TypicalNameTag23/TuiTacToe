using System.Buffers;
using System.Text;
using Common.Utilities;

namespace Common.Packets.Handshake;

public class LoginPacket : BaseAuthenticationPacket
{
    public override PacketType PacketType => PacketType.Login;

    internal LoginPacket() { }
    
    public LoginPacket(string username, byte[] passwordHash) : base(username, passwordHash) { }
}