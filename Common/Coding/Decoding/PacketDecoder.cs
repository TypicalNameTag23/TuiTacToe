using System.Buffers;
using Common.Coding;
using Common.Decoding.ValueDecoders;
using Common.Packets;
using Common.Utilities;

namespace Common.Decoding;

/// <summary>
/// Static utility class to help with packet decoding
/// </summary>
public class PacketDecoder
{
    private readonly Dictionary<PacketType, Func<AbstractPacket>> _packetRegistry;
    private readonly ValueDecoder _decoder;

    public PacketDecoder(
        Dictionary<PacketType, Func<AbstractPacket>> packetRegistry, 
        Dictionary<Type, IValueCoder> coders)
    {
        _packetRegistry = packetRegistry;
        _decoder = new ValueDecoder(coders);
    }
    
    /// <summary>
    /// Tries to read a packet from the provided buffer of bytes
    /// </summary>
    /// <param name="buffer">The buffer to try to read a packet in from</param>
    /// <param name="packet">The packet that was read, null if not able to read packet</param>
    /// <returns>True if a packet was read, otherwise false</returns>
    public int TryReadPacket(byte[] buffer, out AbstractPacket? packet)
    {
        packet = null;
        if (buffer.Length < AbstractPacket.AbstractPacketBaseSize)
        {
            return -1;
        }
        
        var packetType = (PacketType) buffer.GetShort(0);
        if (!_packetRegistry.TryGetValue(packetType, out var packetFactory))
        {
            Console.WriteLine($"Failed to decode packet: unknown packet Id {(short) packetType}");
            return -1;
        }
        
        packet = packetFactory();
        var decodedBytes = _decoder.TryDecodeInto(buffer[sizeof(short)..], packet);
        return decodedBytes + sizeof(short);
    }
}