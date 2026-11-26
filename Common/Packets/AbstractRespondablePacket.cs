using Common.Packets;

namespace Common;

/// <summary>
/// Represents a packet that can be responded to
/// </summary>
public class AbstractRespondablePacket : AbstractPacket
{
    /// <summary>
    /// The id of the packet, used to send a response
    /// </summary>
    public uint Id { get; internal set; }

    // internal ctor for encoding / decoding purposes
    internal AbstractRespondablePacket() { }
    
    public AbstractRespondablePacket(PacketType packetType) : base(packetType) { }
    
    protected override void RegisterValues()
    {
        base.RegisterValues();
        
        RegisterFixedSizeValue(nameof(Id), () => Id, value => Id = value, sizeof(uint));
    }
}