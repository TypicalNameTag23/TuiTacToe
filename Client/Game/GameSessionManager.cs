using Common;
using Common.Packets;
using Common.Packets.Game;
using Common.Packets.Lobby;

namespace Client.Game;

public class GameSessionManager
{
    private readonly ServerConnectionHandler _connectionHandler;

    public bool InGame { get; private set; }
    public int? LobbyCode { get;  private set; }
    public string? OpponentUsername { get; private set; }
    public PlayerType PlayerType { get; private set; }

    public EventHandler<string>? OnOpponentJoin;
    public EventHandler<EventArgs>? OnOpponentDisconnect;
    public EventHandler<(short, short)>? OnBoardUpdateReceived;
    public EventHandler<bool>? OnGameCompleted;

    public GameSessionManager(ServerConnectionHandler connectionHandler)
    {
        _connectionHandler = connectionHandler;
        _connectionHandler.OnConnectionClosed += OnConnectionClosed;
        connectionHandler.OnPacketReceived += OnPacketReceived;
    }

    private void OnConnectionClosed(object? sender, EventArgs e)
    {
        InGame = false;
        LobbyCode = null;
        OpponentUsername = null;
    }

    public async Task<bool> TryCreateGameLobby()
    {
        if (InGame)
        {
            return false;
        }
        
        var packet = new CreateLobbyPacket();
        var response = await _connectionHandler.TrySendAndGetResponseAsync(packet);
        if (response is not { PacketType: PacketType.LobbyCreationInformation })
        {
            return false;
        }

        var lobbyCreationInfo = (LobbyCreationInformationPacket) response;
        LobbyCode = lobbyCreationInfo.LobbyId;
        PlayerType = PlayerType.X;
        InGame = true;
        
        return true;
    }

    public async Task<bool> TryJoinLobby(int code)
    {
        if (InGame)
        {
            return false;
        }
        
        var packet = new JoinLobbyPacket(code);
        var response = await _connectionHandler.TrySendAndGetResponseAsync(packet);
        if (response is not { PacketType: PacketType.LobbyJoinInformation })
        {
            return false;
        }

        var lobbyJoinInfo = (LobbyJoinInformationPacket) response;
        OpponentUsername = lobbyJoinInfo.Username;
        PlayerType = PlayerType.O;
        InGame = true;
        
        return true;
    }

    public async Task<bool> TryLeaveGameLobby()
    {
        if (!InGame)
        {
            return false;
        }
        
        var packet = new LeaveLobbyPacket();
        var response = await _connectionHandler.TrySendAndGetResponseAsync(packet);
        if (response is not { PacketType: PacketType.Accepted })
        {
            return false;
        }
        
        InGame = false;
        LobbyCode = null;
        OpponentUsername = null;
        
        return true;
    }

    public async Task<bool> TryPlayingPosition(short x, short y)
    {
        if (!InGame)
        {
            return false;
        }

        var packet = new BoardPlayedPacket(x, y);
        var response = await _connectionHandler.TrySendAndGetResponseAsync(packet);
        return response is { PacketType: PacketType.Accepted };
    }

    private void OnPacketReceived(object? sender, AbstractPacket packet)
    {
        switch (packet.PacketType)
        {
            case PacketType.OpponentConnected:
            {
                var opponentConnectedPacket = (OpponentConnectedPacket) packet;
                OpponentUsername = opponentConnectedPacket.Username;
                OnOpponentJoin?.Invoke(this, opponentConnectedPacket.Username);
                
                break;
            }
            case PacketType.OpponentDisconnected:
            {
                OnOpponentDisconnect?.Invoke(this, EventArgs.Empty);
                
                break;
            }
            case PacketType.UpdateBoard:
            {
                var updateBoardPacket = (UpdateBoardPacket) packet;
                OnBoardUpdateReceived?.Invoke(this, (updateBoardPacket.X, updateBoardPacket.Y));
                
                break;
            }
            case PacketType.GameCompleted:
            {
                var gameCompletedPacket = (GameCompletedPacket) packet;
                OnGameCompleted?.Invoke(this, gameCompletedPacket.WinnerUsername != OpponentUsername && gameCompletedPacket.WinnerUsername != null);
                InGame = false;
                LobbyCode = null;
                OpponentUsername = null;
                
                break;
            }
        }
    }
}