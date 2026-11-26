namespace Common.Packets.Generic;

public class AcceptedPacket : AbstractResponsePacket
{
    public AbstractPacket? InnerPacket { get; private set; }
    
    internal AcceptedPacket() : base(PacketType.Accepted) { }

    public AcceptedPacket(uint id, AbstractPacket? innerPacket = null) : base(id, PacketType.Accepted)
    {
        InnerPacket = innerPacket;
    }

    protected override void RegisterValues()
    {
        RegisterDynamicSizeValue(nameof(InnerPacket), () => InnerPacket, v => InnerPacket = v);
    }
}