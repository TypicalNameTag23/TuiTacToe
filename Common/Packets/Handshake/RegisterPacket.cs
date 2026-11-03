namespace Common.Packets.Handshake;

public class RegisterPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.Register;
}