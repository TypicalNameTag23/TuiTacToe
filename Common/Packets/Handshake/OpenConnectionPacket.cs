namespace Common.Packets.Handshake;

public class OpenConnectionPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.OpenConnection;
}