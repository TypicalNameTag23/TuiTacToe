namespace Common.Packets;

/// <summary>
/// A response packet
/// </summary>
public abstract class AbstractResponsePacket : AbstractPacket
{
    /// <summary>
    /// The id of the <see cref="AbstractRespondablePacket"/> that is being responded to
    /// </summary>
    public uint Id { get; set; }

    // internal ctor for encoding / decoding purposes
    internal AbstractResponsePacket(PacketType packetType) : base(packetType) { }

    public AbstractResponsePacket(uint id, PacketType packetType) : base(packetType)
    {
        Id = id;
    }
    
    protected override void RegisterValues()
    {
        base.RegisterValues();
        
        RegisterFixedSizeValue(nameof(Id), () => Id, value => Id = value, sizeof(uint));
    }
}