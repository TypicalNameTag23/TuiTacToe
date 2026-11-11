using Common.Decoding.ValueDecoders;
using Common.Packets;
using Common.Packets.Game;
using Common.Packets.Generic;
using Common.Packets.Handshake;
using Common.Packets.Leaderboard;
using Common.Packets.Lobby;

namespace Common.Decoding;

public class PacketDecoderFactory
{
    private Dictionary<PacketType, Func<AbstractPacket>> _registeredPackets = [];
    private Dictionary<Type, IValueDecoder> _registeredDecoders = [];
    
    public static PacketDecoderFactory New()
    {
        return new PacketDecoderFactory();
    }

    public PacketDecoderFactory RegisterPacket(PacketType packetType, Func<AbstractPacket> factory)
    {
        _registeredPackets.Add(packetType, factory);

        return this;
    }

    public PacketDecoderFactory RegisterDecoder(IValueDecoder decoder)
    {
        _registeredDecoders.Add(decoder.Type, decoder);

        return this;
    }
    
    public PacketDecoderFactory WithDefaultPackets()
    {
        #region Initial Handshake
        RegisterPacket(PacketType.OpenConnection, () => new OpenConnectionPacket());
        RegisterPacket(PacketType.Login, () => new LoginPacket());
        RegisterPacket(PacketType.Register, () => new RegisterPacket());
        #endregion

        #region Lobby Management
        RegisterPacket(PacketType.CreateLobby, () => new CreateLobbyPacket());
        RegisterPacket(PacketType.JoinLobby, () => new JoinLobbyPacket());
        RegisterPacket(PacketType.LeaveLobby, () => new LeaveLobbyPacket());
        RegisterPacket(PacketType.LobbyCreationInformation, () => new LobbyCreationInformationPacket()); 
        RegisterPacket(PacketType.LobbyJoinInformation, () => new LobbyJoinInformationPacket()); 
        RegisterPacket(PacketType.OpponentConnected, () => new OpponentConnectedPacket()); 
        RegisterPacket(PacketType.OpponentDisconnected, () => new OpponentDisconnectedPacket()); 
        #endregion

        #region Game Management
        RegisterPacket(PacketType.BoardPlayed, () => new BoardPlayedPacket());
        RegisterPacket(PacketType.UpdateBoard, () => new UpdateBoardPacket());
        RegisterPacket(PacketType.GameCompleted, () => new GameCompletedPacket());
        #endregion

        #region Leaderboard
        RegisterPacket(PacketType.ViewLeaderboard, () => new ViewLeaderboardPacket());
        RegisterPacket(PacketType.LeaderboardInformation, () => new LeaderboardInformationPacket());
        #endregion

        #region General
        RegisterPacket(PacketType.Accepted, () => new AcceptedPacket());
        RegisterPacket(PacketType.Denied, () => new DeniedPacket());
        RegisterPacket(PacketType.CloseConnection, () => new CloseConnectionPacket());
        RegisterPacket(PacketType.Heartbeat, () => new HeartbeatPacket());
        #endregion

        return this;
    }

    public PacketDecoderFactory WithDefaultDecoders()
    {
        RegisterDecoder(new ShortDecoder());
        RegisterDecoder(new IntegerDecoder());
        RegisterDecoder(new LongDecoder());
        RegisterDecoder(new UIntDecoder());
        RegisterDecoder(new ULongDecoder());
        RegisterDecoder(new StringValueDecoder());
        RegisterDecoder(new ByteArrayValueDecoder());

        return this;
    }

    public PacketDecoder Create()
    {
        return new PacketDecoder(_registeredPackets, _registeredDecoders);
    }
}