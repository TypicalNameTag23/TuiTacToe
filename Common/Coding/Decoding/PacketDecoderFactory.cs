using Common.Coding;
using Common.Coding.Coders;
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
    private readonly Dictionary<Type, IValueCoder> _registeredCoders = [];
    private readonly Dictionary<PacketType, Func<AbstractPacket>> _registeredPackets = [];

    public static PacketDecoderFactory New()
    {
        return new PacketDecoderFactory();
    }

    public PacketDecoderFactory WithPacket(PacketType packetType, Func<AbstractPacket> factory)
    {
        _registeredPackets.Add(packetType, factory);

        return this;
    }

    public PacketDecoderFactory WithCoder(IValueCoder coder)
    {
        _registeredCoders.Add(coder.Type, coder);

        return this;
    }

    public PacketDecoderFactory WithDefaultPackets()
    {
        #region Initial Handshake

        WithPacket(PacketType.OpenConnection, () => new OpenConnectionPacket());
        WithPacket(PacketType.Login, () => new LoginPacket());
        WithPacket(PacketType.Register, () => new RegisterPacket());

        #endregion

        #region Lobby Management

        WithPacket(PacketType.CreateLobby, () => new CreateLobbyPacket());
        WithPacket(PacketType.JoinLobby, () => new JoinLobbyPacket());
        WithPacket(PacketType.LeaveLobby, () => new LeaveLobbyPacket());
        WithPacket(PacketType.LobbyCreationInformation, () => new LobbyCreationInformationPacket());
        WithPacket(PacketType.LobbyJoinInformation, () => new LobbyJoinInformationPacket());
        WithPacket(PacketType.OpponentConnected, () => new OpponentConnectedPacket());
        WithPacket(PacketType.OpponentDisconnected, () => new OpponentDisconnectedPacket());

        #endregion

        #region Game Management

        WithPacket(PacketType.BoardPlayed, () => new BoardPlayedPacket());
        WithPacket(PacketType.UpdateBoard, () => new UpdateBoardPacket());
        WithPacket(PacketType.GameCompleted, () => new GameCompletedPacket());

        #endregion

        #region Leaderboard

        WithPacket(PacketType.ViewLeaderboard, () => new ViewLeaderboardPacket());
        WithPacket(PacketType.LeaderboardInformation, () => new LeaderboardInformationPacket());

        #endregion

        #region General

        WithPacket(PacketType.Accepted, () => new AcceptedPacket());
        WithPacket(PacketType.Denied, () => new DeniedPacket());
        WithPacket(PacketType.CloseConnection, () => new CloseConnectionPacket());
        WithPacket(PacketType.Heartbeat, () => new HeartbeatPacket());

        #endregion

        return this;
    }

    public PacketDecoderFactory WithDefaultDecoders()
    {
        WithCoder(new ShortCoder());
        WithCoder(new IntegerCoder());
        WithCoder(new LongCoder());
        WithCoder(new UIntCoder());
        WithCoder(new ULongCoder());
        WithCoder(new StringCoder());
        WithCoder(new ByteArrayCoder());
        WithCoder(new LeaderboardPlacementCoder());
        WithCoder(new AbstractPacketCoder());

        return this;
    }

    public PacketDecoder Create()
    {
        return new PacketDecoder(_registeredPackets, _registeredCoders);
    }
}