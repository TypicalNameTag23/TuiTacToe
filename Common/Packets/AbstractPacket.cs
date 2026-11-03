using System.Buffers;
using Common.Utilities;

namespace Common.Packets;

public abstract class AbstractPacket
{
    // todo hack
    protected static ulong _currentPacketId;
    public const int PacketBaseSizeBytes = PacketTypeSizeBytes + IdSizeBytes;
    public const int PacketTypeSizeBytes = sizeof(short);
    public const int IdSizeBytes = sizeof(ulong);

    public virtual int PacketSizeBytes => PacketBaseSizeBytes;
    
    /// <summary>
    /// The <see cref="PacketType"/>
    /// </summary>
    public abstract PacketType PacketType { get; }
    
    /// <summary>
    /// The id of the packet
    /// </summary>
    public ulong Id { get; set; }

    public AbstractPacket() { }

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
    /// Encodes the <see cref="Message"/> into a byte array
    /// </summary>
    /// <returns>The byte array</returns>
    public virtual byte[] Encode()
    {
        var packetTypeBytes = BitConverter.GetBytes((short) PacketType);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(packetTypeBytes);
        }
        
        var packetIdBytes = BitConverter.GetBytes(Id);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(packetIdBytes);
        }
        
        var encoded = new byte[PacketBaseSizeBytes];
        Buffer.BlockCopy(
            packetTypeBytes, 0, 
            encoded, 0, PacketTypeSizeBytes);
        Buffer.BlockCopy(
            packetIdBytes, 0, 
            encoded, PacketTypeSizeBytes, IdSizeBytes);
        
        return encoded;
    }

    /// <summary>
    /// Reads in data from the buffer and populates the packet
    /// </summary>
    /// <param name="buffer">The buffer to read from</param>
    public virtual void Load(ReadOnlySequence<byte> buffer)
    {
        var slice = buffer
            .Slice(PacketTypeSizeBytes, IdSizeBytes)
            .FirstSpan;
        
        if (BitConverter.IsLittleEndian)
        {
            slice = slice.Reversed();
        }
        
        var data = BitConverter.ToUInt64(slice);
        Id = data;
    }
    
    public override string ToString() => $"{{ PacketType: {PacketType}, Id: {Id} }}";
}