namespace Common.Packets;

/// <summary>
/// Packet abstract base class with common functionality needed by all packets
/// </summary>
public abstract class AbstractPacket
{
    public const int AbstractPacketBaseSize = sizeof(short);
    
    public PacketType PacketType { get; private set; }
    public List<RegisteredValue> RegisteredValues { get; } = [];

    internal AbstractPacket()
    {
        RegisterValues();
    }
    
    public AbstractPacket(PacketType packetType)
    {
        PacketType = packetType;
        
        RegisterValues();
    }
    
    protected virtual void RegisterValues()
    {
        RegisterFixedSizeValue(
            () => PacketType,
            value => PacketType = value,
            sizeof(short));
    }
    
    protected void RegisterFixedSizeValue<T>(Func<T?> getter, Action<T> setter, int sizeBytes)
    {
        RegisteredValues.Add(
            new RegisteredValue(
                () => getter(),
                value => setter((T) value), 
                typeof(T), 
                sizeBytes));
    }
    
    protected void RegisterDynamicSizeValue<T>(Func<T?> getter, Action<T> setter)
    {
        RegisteredValues.Add(
            new RegisteredValue(
                () => getter(),
                value => setter((T) value), 
                typeof(T), 
                null));
    }

    public override string ToString() => this.GetObjectString();
}