namespace Server;

public class GameSession(int lobbyId, Session host)
{
    public EventHandler<GameSessionEndArgs>? OnGameSessionEnd;
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
    }
    
    public void Disconnect(Session session)
    {
        var connectedSession = session == Host ? Guest : Host;
        
        OnGameSessionEnd?.Invoke(this, new GameSessionEndArgs(connectedSession!));
    }
}