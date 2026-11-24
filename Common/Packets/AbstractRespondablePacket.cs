using Common.Packets;

namespace Common;

public class AbstractRespondablePacket : AbstractPacket
{
    public uint Id { get; internal set; }

    internal AbstractRespondablePacket() { }
    
    public AbstractRespondablePacket(PacketType packetType) : base(packetType) { }
    
    protected override void RegisterValues()
    {
        base.RegisterValues();
        
        RegisterFixedSizeValue(nameof(Id), () => Id, value => Id = value, sizeof(uint));
    }
}