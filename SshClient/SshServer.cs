using System.Net;
using Client;
using FxSsh;
using FxSsh.Services;
using SshClient.Ssh.Parsing;
using SshServer;

namespace SshClient;

public class SshServer : IDisposable
{
    private readonly ushort _port;
    private readonly IPAddress _backendAddress;
    private readonly ushort _backendPort;
    
    private readonly FxSsh.SshServer _sshServer;
    private readonly Dictionary<Application, Session> _clientSessionMap = [];

    public SshServer(ushort port, IPAddress backendAddress, ushort backendPort)
    {
        _port = port;
        _backendAddress = backendAddress;
        _backendPort = backendPort;
        
        try
        {
            // jank but whatever
            var directory = Directory.GetCurrentDirectory();
            while (!directory.EndsWith("SshClient") && directory != $"{Path.PathSeparator}")
            {
                directory = Directory.GetParent(directory)!.FullName;
            }
            
            var configReader = new ConfigReader(Path.Join(directory, "config.txt"));
            _sshServer = new FxSsh.SshServer(new StartingInfo(IPAddress.Any, port, "SSH-2.0-tic-tac-toe"));
            _sshServer.AddHostKey("rea-sha2-256", configReader.ReadValue("RSA_KEY"));
            _sshServer.AddHostKey("rsa-sha2-512", configReader.ReadValue("RSA_KEY"));
            _sshServer.AddHostKey("ecdsa-sha2-nistp256", configReader.ReadValue("EDCSA_NISTP256_KEY"));
            _sshServer.AddHostKey("ecdsa-sha2-nistp384", configReader.ReadValue("EDCSA_NISTP384_KEY"));
            _sshServer.AddHostKey("ecdsa-sha2-nistp521", configReader.ReadValue("EDCSA_NISTP521_KEY"));
        }
        catch (Exception e)
        {
            Console.WriteLine("Failed access keys from the config file, did you create it using the script?");
            throw e;
        }

        _sshServer.ConnectionAccepted += OnConnectionAccepted;
    }

    public void Start()
    {
        _sshServer.Start();
        Console.WriteLine($"Server up and listening on port {_port}...");
    }

    public void Stop()
    {
        Console.WriteLine("Stopping server...");
        foreach (var client in _clientSessionMap.Keys)
        {
            client.DisconnectAsync().Wait();
        }
    }
    
    private void OnConnectionAccepted(object? sender, Session session)
    {
        Console.WriteLine($"Connection accepted");

        session.ServiceRegistered += ServiceRegistered;
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
        
        var terminal = new SshTerminal(e, new XTermInputParser());
        var client = new Application(terminal, _backendAddress, _backendPort);
        client.OnDisconnect += OnClientDisconnect;
        
        _ = client.StartAsync();
        _clientSessionMap[client] = session;
    }

    private void OnClientDisconnect(object? sender, EventArgs e)
    {
        var client = (Application) sender!;
        
        _clientSessionMap[client].Disconnect();
        _clientSessionMap.Remove(client);
    }

    public void Dispose()
    {
        _sshServer.Dispose();
    }
}