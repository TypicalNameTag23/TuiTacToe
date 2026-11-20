using Common;

namespace Server;

public class GameSession(int lobbyId, Session host)
{
    private PlayerType?[,] _board =  new PlayerType?[3,3];
    private Session _currentPlayer = host;
    
    public EventHandler<Session?>? OnGameCompleted;
    public EventHandler<EventArgs>? OnOpponentDisconnected;
    public EventHandler<Session>? OnGuestConnected;
    public EventHandler<(Session, short, short)>? OnBoardUpdated;
    
    public Session? Host { get; private set; } = host;
    public Session? Guest { get; private set; }
    
    public int LobbyId { get; } = lobbyId;

    public void Connect(Session session)
    {
        if (Guest != null)
        {
            throw new InvalidOperationException("There is already a guest connected.");
        }
        
        Guest = session;
        OnGuestConnected?.Invoke(this, session);
    }
    
    public void Disconnect(Session session)
    {
        if (Guest == null)
        {
            return;
        }
        
        var connectedSession = session == Host ? Guest : Host;

        OnOpponentDisconnected?.Invoke(this, EventArgs.Empty);
        OnGameCompleted?.Invoke(this, connectedSession);
    }

    public bool PlayPosition(Session player, short x, short y)
    {
        // lovely
        if (Guest == null || Host == null || _currentPlayer != player || x < 0 || x > 2 || y < 0 || y > 2 || _board[x,y] != null)
        {
            return false;
        }

        _board[x,y] = player == Host ? PlayerType.X : PlayerType.O;
        OnBoardUpdated?.Invoke(this, (player, x, y));
        CheckForCompletion();
        
        _currentPlayer = player == Host ? Guest : Host;
        return true;
    }
    
    private void CheckForCompletion()
    {
        var isBoardFilled = _board
            .Cast<PlayerType?>()
            .All(position => position != null);

        if (isBoardFilled)
        {
            OnGameCompleted?.Invoke(this, null);
        }
         
        for (var i = 0; i < 3; i++)
        {
            if (_board[i,0] != null && _board[i,1] == _board[i,0] && _board[i,2] == _board[i,0])
            {
                OnGameCompleted?.Invoke(this, _currentPlayer);
            }
             
            if (_board[0,i] != null && _board[1,i] == _board[0,i] && _board[2,i] == _board[0,i])
            {
                OnGameCompleted?.Invoke(this, _currentPlayer);
            }
        }

        if (_board[0,0] != null && _board[1,1] == _board[0,0] && _board[2,2] == _board[0,0])
        {
            OnGameCompleted?.Invoke(this, _currentPlayer);
        }

        if (_board[0,2] != null && _board[1,1] == _board[0,2] && _board[2,0] == _board[0,2])
        {
            OnGameCompleted?.Invoke(this, _currentPlayer);
        }
    }
}