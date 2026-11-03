namespace Common.Packets.Generic;

public class HeartbeatPacket : AbstractPacket
{
    public override PacketType PacketType => PacketType.Heartbeat;
}