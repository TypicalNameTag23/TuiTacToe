using System.Text;
using Common.Coding;

namespace Common.Packets;

/// <summary>
/// Base packet
/// </summary>
public abstract class AbstractPacket : Codable
{
    public const int AbstractPacketBaseSize = sizeof(short);

    // internal ctor for encoding / decoding purposes
    internal AbstractPacket() { }

    public AbstractPacket(PacketType packetType)
    {
        PacketType = packetType;
    }
    
    /// <summary>
    /// The type of the packet
    /// </summary>
    public PacketType PacketType { get; private set; }

    public override string ToString()
    {
        var sb = new StringBuilder(PacketType.ToString());

        sb.Append(" { ");

        for (var i = 0; i < CodableValues.Count; i++)
        {
            var registeredValue = CodableValues[i];
            if (registeredValue.FieldName == nameof(PacketType))
            {
                continue;
            }

            sb.Append($"{registeredValue.FieldName}: {registeredValue.Value}");
            if (i < CodableValues.Count - 1)
            {
                sb.Append(", ");
            }
        }

        sb.Append(" }");
        return sb.ToString();
    }
}