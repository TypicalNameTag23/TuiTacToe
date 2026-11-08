using System.Net;
using System.Net.Sockets;
using Common;
using Server.Data;
using Server.Data.Models;

namespace Server;

public class Server : IDisposable
{
    private const int PollTimeoutMillis = 10;
    private const int Port = 4343;

    private TcpListener _listener;
    private readonly List<Session> _sessions = [];
    private readonly Dictionary<int, GameSession> _gameSessionMap = [];
    
    private ILeaderboardSource _leaderboardSource = new LeaderboardSource();
    private IUserDatabase _userDatabase = new UserDatabase();
    private Random _random = new();

    public bool Running { get; private set; }

    public void Start()
    {
        Running = true;
        _listener = new TcpListener(IPAddress.Any, Port);
        _listener.Start();
        Console.WriteLine($"Server running and listening on port {Port}...");

        while (Running)
        {
            Poll();
            Thread.Sleep(PollTimeoutMillis);
        }
    }

    public void Stop()
    {
        _listener.Stop();
        Running = false;
        
        _sessions.ForEach(it =>
        {
            it.OnDisconnect -= OnSessionDisconnected;
            it.Disconnect();
            it.Dispose();
        });
    }

    private void Poll()
    {
        while (_listener.Pending())
        {
            var client = _listener.AcceptTcpClient();
            var session = new Session(
                client, 
                _leaderboardSource, 
                GetGameSession,
                TryCreateUserAsync,
                ValidatePasswordAsync);
            session.OnDisconnect += OnSessionDisconnected;
            
            _sessions.Add(session);
            _ = session.StartAsync();

            Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
        }
    }

    private async Task<User?> ValidatePasswordAsync(string username, string password)
    {
        try
        {
            var user = await _userDatabase.GetUserAsync(username);
            if (user == null)
            {
                return null;
            }
            
            return !HashHelper.ValidatePassword(password, user.passwordHash) ? null : user;
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to create user: " + username);
            Console.WriteLine(e);

            return null;
        }
    }

    private async Task<User?> TryCreateUserAsync(string username, string password)
    {
        var user = new User(username, password);
        try
        {
            if (!await _userDatabase.CreateUserAsync(user))
            {
                return null;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed to create user: " + username);
            Console.WriteLine(e);

            return null;
        }
        
        return user;
    }

    private GameSession GetGameSession(Session session)
    {
        if (_gameSessionMap.Count >= 999_990_000)
        {
            Console.WriteLine("womp womp");
            throw new Exception("womp womp");
        }
        
        var lobbyId = _random.Next(10_000, 1_000_000_000);
        while (_gameSessionMap.ContainsKey(lobbyId))
        {
            lobbyId = _random.Next(10000, 99999);
        }
        
        return new GameSession(lobbyId, session);
    }

    private void OnSessionDisconnected(object? sender, EventArgs args)
    {
        var session = (Session) sender!;
        
        _sessions.Remove(session);
        session.Dispose();
    }

    public void Dispose()
    {
        _listener.Dispose();
    }
}