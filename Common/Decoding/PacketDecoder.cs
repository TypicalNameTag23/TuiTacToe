using System.Buffers;
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
    private readonly Dictionary<Type, IValueDecoder> _valueDecoders;

    public PacketDecoder(
        Dictionary<PacketType, Func<AbstractPacket>> packetRegistry, 
        Dictionary<Type, IValueDecoder> valueDecoders)
    {
        _packetRegistry = packetRegistry;
        _valueDecoders = valueDecoders;
    }
    
    /// <summary>
    /// Tries to read a packet from the provided buffer of bytes
    /// </summary>
    /// <param name="buffer">The buffer to try to read a packet in from</param>
    /// <param name="packet">The packet that was read, null if not able to read packet</param>
    /// <returns>True if a packet was read, otherwise false</returns>
    public int TryReadPacket(ReadOnlySequence<byte> buffer, out AbstractPacket? packet)
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
        var byteOffset = AbstractPacket.AbstractPacketBaseSize;
        foreach (var registeredValue in packet.RegisteredValues)
        {
            if (registeredValue.Type == typeof(PacketType))
            {
                registeredValue.Value = packetType;
                continue;
            }

            if (!_valueDecoders.TryGetValue(registeredValue.Type, out var valueDecoder))
            {
                throw new Exception($"Unable to find decoder for type {registeredValue.Type}");
            }

            var valueSizeBytes = registeredValue.SizeBytes;
            if (!registeredValue.IsFixedSize)
            {
                valueSizeBytes = buffer.GetInt(byteOffset);
                byteOffset += sizeof(int);
            }
            
            registeredValue.Value = valueDecoder.Decode(
                buffer.Slice(byteOffset, (int) valueSizeBytes!).FirstSpan);
            byteOffset += (int) valueSizeBytes;
        }
        
        return byteOffset;
    }
    
    
    // public static bool TryReadPacket(ref ReadOnlySequence<byte> buffer, out AbstractPacket? packet)
    // {
    //     packet = null;
    //     if (buffer.Length < AbstractPacket.PacketTypeSizeBytes)
    //     {
    //         return false;
    //     }
    //     
    //     try
    //     {
    //         var slice = buffer
    //             .Slice(0, AbstractPacket.PacketTypeSizeBytes)
    //             .FirstSpan;
    //         
    //         if (BitConverter.IsLittleEndian)
    //         {
    //             slice = slice.Reversed();
    //         }
    //         
    //         var packetTypeBytes = BitConverter.ToInt16(slice);
    //         var packetType = (PacketType) packetTypeBytes;
    //         if (!PacketRegistry.TryGetValue(packetType, out var packetFactory))
    //         {
    //             Console.WriteLine($"Failed to decode packet: unknown packet Id {packetTypeBytes}");
    //             return false;
    //         }
    //
    //         packet = packetFactory();
    //         if (!packet.Load(buffer))
    //         {
    //             return false;
    //         }
    //         
    //         buffer = buffer.Slice(packet.PacketSizeBytes);
    //         
    //         return true;
    //     }
    //     catch (EndOfStreamException e)
    //     {
    //         Console.WriteLine(e);
    //         return false;
    //     }
    //     catch (IOException e)
    //     {
    //         Console.WriteLine(e);
    //         return false;
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         return false;
    //     }
    // }
}