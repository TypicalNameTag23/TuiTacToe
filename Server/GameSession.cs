namespace Server;

public class GameSession(int lobbyId, Session host)
{
    public EventHandler<Session>? OnGameCompleted;
    public EventHandler<EventArgs>? OnOpponentDisconnected;
    public EventHandler<Session>? OnGuestConnected;
    
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
        DeclareWinner(connectedSession!);
    }

    public bool PlayPosition(short x, short y)
    {
        return true;
    }

    private void DeclareWinner(Session winner)
    {
        OnGameCompleted?.Invoke(this, winner);
    }
}