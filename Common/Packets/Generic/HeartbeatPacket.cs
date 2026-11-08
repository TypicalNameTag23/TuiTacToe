namespace Common.Packets.Generic;

public class HeartbeatPacket : AbstractPacket
{
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    public override PacketType PacketType => PacketType.Heartbeat;
}