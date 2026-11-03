namespace Common.Packets.Handshake;

public class LoginInformation : AbstractPacket
{
    public override PacketType PacketType => PacketType.LoginInformation;
}