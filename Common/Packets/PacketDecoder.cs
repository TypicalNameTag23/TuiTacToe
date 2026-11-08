using System.Buffers;
using Common.Packets.Game;
using Common.Packets.Generic;
using Common.Packets.Handshake;
using Common.Packets.Leaderboard;
using Common.Packets.Lobby;
using Common.Utilities;

namespace Common.Packets;

/// <summary>
/// Static utility class to help with packet decoding
/// </summary>
public static class PacketDecoder
{
    private static readonly Dictionary<PacketType, Func<AbstractPacket>> PacketRegistry = new()
    {
        #region Initial Handshake
        [PacketType.OpenConnection] = () => new OpenConnectionPacket(),
        [PacketType.Login] = () => new LoginPacket(),
        [PacketType.Register] = () => new RegisterPacket(),
        [PacketType.LoginInformation] = () => new LoginInformation(),
        #endregion

        #region Lobby Management
        [PacketType.CreateLobby] = () => new CreateLobbyPacket(),
        [PacketType.JoinLobby] = () => new JoinLobbyPacket(),
        [PacketType.LeaveLobby] = () => new LeaveLobbyPacket(),
        [PacketType.LobbyCreationInformation] = () => new LobbyCreationInformationPacket(), 
        [PacketType.LobbyJoinInformation] = () => new LobbyJoinInformationPacket(), 
        [PacketType.OpponentConnected] = () => new OpponentConnectedPacket(), 
        [PacketType.OpponentDisconnected] = () => new OpponentDisconnectedPacket(), 
        #endregion

        #region Game Management
        [PacketType.BoardPlayed] = () => new BoardPlayedPacket(),
        [PacketType.UpdateBoard] = () => new UpdateBoardPacket(),
        [PacketType.GameCompleted] = () => new GameCompletedPacket(),
        #endregion

        #region Leaderboard
        [PacketType.ViewLeaderboard] = () => new ViewLeaderboardPacket(),
        [PacketType.LeaderboardInformation] = () => new LeaderboardInformationPacket(),
        #endregion

        #region General
        [PacketType.Accepted] = () => new AcceptedPacket(),
        [PacketType.Denied] = () => new DeniedPacket(),
        [PacketType.CloseConnection] = () => new CloseConnectionPacket(),
        [PacketType.Heartbeat] = () => new HeartbeatPacket(),
        #endregion
    };

    /// <summary>
    /// Tries to read a packet from the provided buffer of bytes
    /// </summary>
    /// <param name="buffer">The buffer to try to read a packet in from</param>
    /// <param name="packet">The packet that was read, null if not able to read packet</param>
    /// <returns>True if a packet was read, otherwise false</returns>
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
            
            var packetTypeBytes = BitConverter.ToInt16(slice);
            var packetType = (PacketType) packetTypeBytes;
            if (!PacketRegistry.TryGetValue(packetType, out var packetFactory))
            {
                Console.WriteLine($"Failed to decode packet: unknown packet Id {packetTypeBytes}");
                return false;
            }

            packet = packetFactory();
            if (!packet.Load(buffer))
            {
                return false;
            }
            
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