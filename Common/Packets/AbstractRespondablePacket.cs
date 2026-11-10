using Common.Packets;

namespace Common;

public class AbstractRespondablePacket : AbstractPacket
{
    public uint Id;

    internal AbstractRespondablePacket() { }
    
    public AbstractRespondablePacket(PacketType packetType) : base(packetType) { }
    
    protected override void RegisterValues()
    {
        base.RegisterValues();
        
        RegisterFixedSizeValue(() => Id, value => Id = value, sizeof(uint));
    }
}