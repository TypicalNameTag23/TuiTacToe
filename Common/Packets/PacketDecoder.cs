using System.Buffers;
using Common.Packets.Generic;
using Common.Packets.Lobby;
using Common.Utilities;

namespace Common.Packets;

public static class PacketDecoder
{
    private static readonly Dictionary<PacketType, Func<AbstractPacket>> PacketRegistry = new()
    {
        [PacketType.LobbyCreate] = () => new LobbyCreatePacket(),
        [PacketType.LobbyJoin] = () => new LobbyJoinPacket(),
        [PacketType.LobbyInfo] = () => new LobbyInfoPacket(), //client side
        [PacketType.Denied] = () => new DeniedPacket(),
        [PacketType.CloseConnection] = () => new CloseConnectionPacket(),
        [PacketType.ViewLeaderboard] = () => new ViewLeaderboardPacket(),
    };

    public static bool TryReadPacket(ref ReadOnlySequence<byte> buffer, out AbstractPacket? packet)
    {
        packet = null;
        if (buffer.Length < AbstractPacket.PacketTypeSizeBytes)
        {
            return false;
        }
        
        try
        {
            var slice = buffer
                .Slice(0, AbstractPacket.PacketTypeSizeBytes)
                .FirstSpan;
            
            if (BitConverter.IsLittleEndian)
            {
                slice = slice.Reversed();
            }
            
            var packetId = BitConverter.ToInt16(slice);
            var packetType = (PacketType) packetId;
            if (!PacketRegistry.TryGetValue(packetType, out var packetFactory))
            {
                Console.WriteLine($"Failed to decode packet: unknown packet Id {packetId}");
                return false;
            }

            packet = packetFactory();
            if (buffer.Length < packet.PacketSizeBytes)
            {
                return false;
            }
            
            packet.Load(buffer);
            buffer = buffer.Slice(packet.PacketSizeBytes);
            
            return true;
        }
        catch (EndOfStreamException e)
        {
            Console.WriteLine(e);
            return false;
        }
        catch (IOException e)
        {
            Console.WriteLine(e);
            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}