namespace Common.Packets.Handshake;

public class LoginPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.Login;
}