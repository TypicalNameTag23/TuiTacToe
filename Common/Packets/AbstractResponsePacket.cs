namespace Common.Packets;

public abstract class AbstractResponsePacket : AbstractPacket
{
    public uint Id { get; set; }

    internal AbstractResponsePacket(PacketType packetType) : base(packetType) { }

    public AbstractResponsePacket(uint id, PacketType packetType) : base(packetType)
    {
        Id = id;
    }
    
    protected override void RegisterValues()
    {
        base.RegisterValues();
        
        RegisterFixedSizeValue(() => Id, value => Id = value, sizeof(uint));
    }
}