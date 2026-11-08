using System.Buffers;
using Common.Utilities;

namespace Common.Packets;

/// <summary>
/// Packet abstract base class with common functionality needed by all packets
/// </summary>
public abstract class AbstractPacket
{
    // todo hack
    protected static ulong _currentPacketId;
    public const int PacketBaseSizeBytes = PacketTypeSizeBytes + IdSizeBytes;
    public const int PacketTypeSizeBytes = sizeof(short);
    public const int IdSizeBytes = sizeof(ulong);

    /// <summary>
    /// The size of the packet in bytes
    /// </summary>
    /// todo change to bits, not everything is byte aligned
    public virtual int PacketSizeBytes => PacketBaseSizeBytes;
    
    /// <summary>
    /// The <see cref="PacketType"/>
    /// </summary>
    public abstract PacketType PacketType { get; }
    
    /// <summary>
    /// The id of the packet
    /// </summary>
    public ulong Id { get; private set; }

    public AbstractPacket()
    {
        Id = NextPacketId();
    }

    public AbstractPacket(ulong id)
    {
        Id = id;
    }

    public static ulong NextPacketId()
    {
        _currentPacketId++;
        return _currentPacketId - 1;
    }
    
    /// <summary>
    /// Encodes this packet into a byte array
    /// </summary>
    /// <returns>The byte array</returns>
    public virtual byte[] Encode()
    {
        var packetTypeBytes = BitConverters.GetBytesEndianIndependent((short) PacketType);
        var packetIdBytes = BitConverters.GetBytesEndianIndependent(Id);
        
        var encoded = new byte[PacketBaseSizeBytes];
        encoded.CopyInto(packetTypeBytes, packetIdBytes);
        return encoded;
    }

    /// <summary>
    /// Reads in data from the buffer and populates the packet
    /// </summary>
    /// <param name="buffer">The buffer to read from</param>
    internal virtual bool Load(ReadOnlySequence<byte> buffer)
    {
        if (buffer.Length < PacketBaseSizeBytes)
        {
            return false;
        }
        
        Id = buffer.GetULong(PacketTypeSizeBytes, IdSizeBytes);
        
        return true;
    }

    public override string ToString() => this.GetObjectString();
}