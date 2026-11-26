using Common.Packets.Game;
using Common.Packets.Generic;
using Common.Packets.Lobby;

namespace Server;

public partial class Session
{
    private readonly Func<int, GameSession?> _getGameSession;
    private readonly Func<Session, GameSession> _createGameSession;
    
    public GameSession? GameSession { get; private set; }

    private async Task HandleBoardPlayedAsync(BoardPlayedPacket packet)
    {
        if (!Authenticated || GameSession is null || !GameSession.PlayPosition(this, packet.X, packet.Y))
        {
            await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id));
            return;
        }
          
        await _packetHandler.TrySendAsync(new AcceptedPacket(packet.Id));
    }

    private async Task JoinLobbyAsync(JoinLobbyPacket packet)
    {
        if (!Authenticated || GameSession != null)
        {
            await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id));
            return;
        }
          
        GameSession = _getGameSession(packet.LobbyId);
        if (GameSession == null)
        {
            await _packetHandler.TrySendAsync(new DeniedPacket(packet.Id));
            return;
        }

        GameSession.Connect(this);
        GameSession.OnGameCompleted += OnGameCompletedAsync; 
        GameSession.OnOpponentDisconnected += OnOpponentDisconnectedAsync; 
        GameSession.OnBoardUpdated += OnBoardUpdatedAsync;

        var responsePacket = new LobbyJoinInformationPacket(GameSession.Host!.User!.Username, packet.Id);
        await _packetHandler.TrySendAsync(responsePacket);
          
        throw new NotImplementedException();
    }

    private async Task HandleLeaveLobbyPacketAsync(uint id)
    {
        if (!Authenticated || GameSession == null)
        {
            await _packetHandler.TrySendAsync(new DeniedPacket(id));
            return;
        }
          
        GameSession.OnGameCompleted -= OnGameCompletedAsync; 
        GameSession.OnGuestConnected -= OnOpponentConnectedAsync; 
        GameSession.OnOpponentDisconnected -= OnOpponentDisconnectedAsync; 
        GameSession.Disconnect(this);
        GameSession = null;
        await _packetHandler.TrySendAsync(new AcceptedPacket(id));
    }

    private async Task CreateLobbyAsync(uint id)
    { 
        if (!Authenticated || GameSession != null)
        {
            await _packetHandler.TrySendAsync(new DeniedPacket(id));
            return;
        }
                    
        GameSession = _createGameSession(this);
        GameSession.OnGameCompleted += OnGameCompletedAsync; 
        GameSession.OnGuestConnected += OnOpponentConnectedAsync; 
        GameSession.OnOpponentDisconnected += OnOpponentDisconnectedAsync; 
        GameSession.OnBoardUpdated += OnBoardUpdatedAsync;

        var packet = new LobbyCreationInformationPacket(GameSession.LobbyId, id);
        await _packetHandler.TrySendAsync(packet);
    }

    private async void OnBoardUpdatedAsync(object? sender, (Session, short, short) e)
    {
        if (e.Item1 == this)
        {
            return;
        }

        await _packetHandler.TrySendAsync(new UpdateBoardPacket(e.Item2, e.Item3));
    }

    private async void OnOpponentConnectedAsync(object? sender, Session session)
     {
          await _packetHandler.TrySendAsync(new OpponentConnectedPacket(session.User!.Username));
     }

    private async void OnOpponentDisconnectedAsync(object? sender, EventArgs e)
     {
          await _packetHandler.TrySendAsync(new OpponentDisconnectedPacket());
     }

    private async void OnGameCompletedAsync(object? sender, Session? winner)
     {
          await _packetHandler.TrySendAsync(new GameCompletedPacket(winner?.User?.Username));
          if (winner == this)
          {
               User!.Wins += 1;
               await _tryUpdateUserAsync(User);
          }
          
          GameSession?.OnGameCompleted -= OnGameCompletedAsync; 
          GameSession?.OnGuestConnected -= OnOpponentConnectedAsync; 
          GameSession?.OnOpponentDisconnected -= OnOpponentDisconnectedAsync; 
          GameSession = null;
     }
}