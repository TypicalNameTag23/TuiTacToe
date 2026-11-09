using System.Net;
using FxSsh;
using FxSsh.Services;

namespace Client.Ssh;

public class SshServer
{
    private const int Port = 22;
    private static readonly string ConfigFilePath = Path.Join(".", "..", "..", "..", "config.txt");

    private readonly FxSsh.SshServer _sshServer;
    private readonly List<Game.Client> _clients = [];

    public SshServer()
    {
        try
        {
            var configReader = new ConfigReader(ConfigFilePath);
            _sshServer = new FxSsh.SshServer(new StartingInfo(IPAddress.Any, Port, "SSH-2.0-tic-tac-toe"));
            _sshServer.AddHostKey("rea-sha2-256", configReader.ReadValue("RSA_KEY"));
            _sshServer.AddHostKey("rsa-sha2-512", configReader.ReadValue("RSA_KEY"));
            _sshServer.AddHostKey("ecdsa-sha2-nistp256", configReader.ReadValue("EDCSA_NISTP256_KEY"));
            _sshServer.AddHostKey("ecdsa-sha2-nistp384", configReader.ReadValue("EDCSA_NISTP384_KEY"));
            _sshServer.AddHostKey("ecdsa-sha2-nistp521", configReader.ReadValue("EDCSA_NISTP521_KEY"));
        }
        catch (Exception)
        {
            throw new Exception("Failed access keys from the config file, did you create it using the script?");
        }

        _sshServer.ConnectionAccepted += OnConnectionAccepted;
    }

    public void Start()
    {
        _sshServer.Start();
        Console.WriteLine($"Server up and listening on port {Port}...");
    }

    public void Stop()
    {
        _clients.ForEach(it =>
        {
            // it.DisconnectAsync().Wait();
        });
    }
    
    private void OnConnectionAccepted(object? sender, Session session)
    {
        Console.WriteLine($"Client connection accepted");

        session.ServiceRegistered += ServiceRegistered;
        session.ConfigureKeepalive(TimeSpan.Zero);
    }

    private void ServiceRegistered(object? sender, SshService e)
    {
        if (sender is null)
        {
            return; 
        }
        
        var session = (Session) sender;
        Console.WriteLine($"Session {Convert.ToHexString(session.SessionId).Replace("-", "")} requesting service: {e.GetType().Name}.");
        switch (e)
        {
            case UserAuthService userAuthService:
            {
                userAuthService.EnableNoneAuth = true;
                userAuthService.UserAuth += UserAuth;
                break;
            }
            case ConnectionService connectionService:
            {
                connectionService.PtyReceived += (_, a) => PtyReceived(a, session);
                connectionService.CommandOpened += (_, args) => args.Agreed = true;
                break;
            }
        }
    }

    private void UserAuth(object? sender, UserAuthArgs e)
    {
        e.Result = true;
    }

    private void PtyReceived(PtyArgs e, Session session)
    {
        Console.WriteLine($"Session {Convert.ToHexString(session.SessionId).Replace("-", "")} requesting new pseudo terminal");
        
        var client = new Game.Client(e, session);
        _ = client.StartAsync();
        
        _clients.Add(client);
    }
}