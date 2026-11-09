namespace Common.Packets.Generic;

public class DeniedPacket : AbstractPacket
{
    public DeniedPacket() { }
    
    public DeniedPacket(ulong id) : base(id) { }
    
    public override int PacketSizeBytes => PacketBaseSizeBytes;
    
    public override PacketType PacketType => PacketType.Denied;
}