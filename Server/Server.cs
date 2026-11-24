using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Common.Utilities;
using Server.Data;
using Server.Data.Models;

namespace Server;

public class Server(int port, X509Certificate2 certificate) : IDisposable
{
    private const int PollTimeoutMillis = 10;

    private TcpListener _listener;
    private readonly List<Session> _sessions = [];
    private readonly Dictionary<int, GameSession> _gameSessionMap = [];
    
    private ILeaderboardSource _leaderboardSource = new LeaderboardSource();
    private IUserDatabase _userDatabase = new InMemoryUserDatabase();
    private Random _random = new();

    public bool Running { get; private set; }

    public void Start()
    {
        Running = true;
        _listener = new TcpListener(IPAddress.Any, port);
        _listener.Start();
        Console.WriteLine($"Server running and listening on port {port}...");

        while (Running)
        {
            Poll();
        }
    }

    public async Task StopAsync()
    {
        _listener.Stop();
        Running = false;
        
        foreach (var session in _sessions)
        {
            session.OnDisconnect -= OnSessionDisconnected;
            await session.DisconnectAsync();
            session.Dispose();
        }
    }

    private void Poll()
    {
        while (_listener.Pending())
        {
            var client = _listener.AcceptTcpClient();
            var ssl = new SslStream(client.GetStream());
            ssl.AuthenticateAsServer(certificate);
            
            Console.WriteLine($"Server: {ssl.SslProtocol}, {ssl.NegotiatedCipherSuite}");
            
            var session = new Session(
                ssl, 
                _leaderboardSource, 
                CreateGameSession,
                GetGameSession,
                TryCreateUserAsync,
                ValidatePasswordAsync);
            session.OnDisconnect += OnSessionDisconnected;
            _ = session.StartAsync();

            _sessions.Add(session);
            Console.WriteLine($"Client {client.Client.RemoteEndPoint} connected");
        }
    }

    private GameSession? GetGameSession(int lobbyId)
    {
        if (!_gameSessionMap.TryGetValue(lobbyId, out var gameSession) || gameSession.Guest != null)
        {
            return null;
        }
        
        return gameSession;
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
            
            var hashedPassword = HashHelper.HashPassword(password, Convert.FromBase64String(user.Salt));
            Console.WriteLine(Convert.ToBase64String(hashedPassword));
            Console.WriteLine(user.PasswordHash);
            var a = CryptographicOperations.FixedTimeEquals(hashedPassword,
                Convert.FromBase64String(user.PasswordHash));
            Console.WriteLine(a);
            return a ? user : null;
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
        var hashedPassword = HashHelper.HashPassword(password, out var salt);
        var passwordBase64 = Convert.ToBase64String(hashedPassword);
        var saltBase64 = Convert.ToBase64String(salt);
        
        var user = new User(username, passwordBase64, saltBase64);
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

    private GameSession CreateGameSession(Session session)
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

        var gameSession = new GameSession(lobbyId, session);
        gameSession.OnGameCompleted += OnGameCompleted;
        _gameSessionMap.Add(lobbyId, gameSession);
        
        return gameSession;
    }

    private void OnGameCompleted(object? sender, Session e)
    {
        var gameSession = sender as GameSession;
        _gameSessionMap.Remove(gameSession!.LobbyId);
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