using Common.Packets;
using Common.Packets.Lobby;

namespace Client.Game;

public class GameSessionManager
{
    private readonly ServerConnectionHandler _connectionHandler;
    private TicTacToeGame? _game;

    public GameSessionManager(ServerConnectionHandler connectionHandler)
    {
        _connectionHandler = connectionHandler;
        connectionHandler.OnPacketReceived += OnPacketReceived;
    }

    public async Task<bool> TryCreateGameLobby()
    {
        if (_game == null)
        {
            return false;
        }
        
        var packet = new CreateLobbyPacket();
        var response = await _connectionHandler.TrySendAndGetResponseAsync(packet);
        if (response is not { PacketType: PacketType.LobbyCreationInformation })
        {
            // view.EnableButtons();
            return false;
        }

        return false;
    }

    private void OnPacketReceived(object? sender, AbstractPacket packet)
    {
        switch (packet.PacketType)
        {
            case PacketType.OpponentConnected:
            {
                break;
            }
            case PacketType.OpponentDisconnected:
            {
                break;
            }
            case PacketType.BoardPlayed:
            {
                break;
            }
            case PacketType.UpdateBoard:
            {
                break;
            }
            case PacketType.GameCompleted:
            {
                break;
            }
        }
    }
}