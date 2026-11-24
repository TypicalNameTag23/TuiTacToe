using System.Text;

namespace Common.Packets;

/// <summary>
/// Packet abstract base class with common functionality needed by all packets
/// </summary>
public abstract class AbstractPacket
{
    public const int AbstractPacketBaseSize = sizeof(short);
    
    internal readonly List<RegisteredValue> RegisteredValues = [];

    public PacketType PacketType { get; private set; }

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
            nameof(PacketType),
            () => PacketType,
            value => PacketType = value,
            sizeof(short));
    }
    
    protected void RegisterFixedSizeValue<T>(string name, Func<T?> getter, Action<T> setter, int sizeBytes)
    {
        RegisteredValues.Add(
            new RegisteredValue(
                name,
                () => getter(),
                value => setter((T) value), 
                typeof(T), 
                sizeBytes));
    }
    
    protected void RegisterDynamicSizeValue<T>(string name, Func<T?> getter, Action<T> setter)
    {
        RegisteredValues.Add(
            new RegisteredValue(
                name,
                () => getter(),
                value => setter((T) value), 
                typeof(T), 
                null));
    }

    public override string ToString()
    {
        var sb = new StringBuilder(PacketType.ToString());

        sb.Append(" { ");
        
        for (var i = 0; i < RegisteredValues.Count; i++)
        {
            var registeredValue = RegisteredValues[i]; 
            if (registeredValue.FieldName == nameof(PacketType))
            {
                continue;
            }
            
            sb.Append($"{registeredValue.FieldName}: {registeredValue.Value}");
            if (i < RegisteredValues.Count - 1)
            {
                sb.Append(", ");
            }
        }
        
        sb.Append(" }");
        return sb.ToString();
    }
}