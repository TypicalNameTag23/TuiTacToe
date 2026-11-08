using Client.Game.Drawables;
using Client.Game.Views;
using Client.Ssh;
using Client.Ssh.Parsing;
using Common;
using Common.Packets;
using Common.Packets.Handshake;
using Common.Packets.Leaderboard;
using Common.Packets.Lobby;
using FxSsh;
using FxSsh.Services;
using Painter;
using Painter.Drawables;

namespace Client.Game;

public class Client : IDisposable
{
    private static readonly Key CtrlCKey = new(ConsoleKey.C, ConsoleModifiers.Control);
    private const string BackendServerAddress = "127.0.0.1";
    private const int BackendServerPort = 4343;

    private DisconnectedView _disconnectedView;
    private LoginView _loginView;
    private ContentControl _contentDisplay;
    private MainPageView _mainPageView;
    private Application _app;

    private readonly Session _session;
    private readonly PtyArgs _args;
    
    private CancellationTokenSource _cancellationTokenSource;
    private ServerConnectionHandler _serverConnectionHandler;

    public Client(PtyArgs args, Session session)
    {
        _args = args;
        _session = session;
    }

    public async Task StartAsync()
    {
        #region Setup application views
        _loginView = new LoginView(OnLoginButtonClick, OnSignUpButtonClick);
        _mainPageView = new MainPageView(OnCreateLobbyClick, OnJoinLobbyClick, OnLeaderboardClick);
        _disconnectedView = new DisconnectedView(OnReconnectClick);
        _contentDisplay = new ContentControl
        {
            Content = _loginView
        };
        #endregion

        #region Setup application
        var terminal = new SshTerminal(_args, new XTermInputParser());
        _app = new Application(terminal)
        {
            Content = new Grid { RowDefinitions = RowDefinition.Of("1, *") }
                .SetContent(new FpsTacker())
                .SetContent(_contentDisplay, row: 1)
        };
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { DisconnectAsync().Wait(); });
        #endregion

        #region Setup server connection
        _serverConnectionHandler = new ServerConnectionHandler(BackendServerAddress, BackendServerPort);
        _serverConnectionHandler.OnConnectionClosed += (_, _) => OnDisconnectedFromServer();

        _cancellationTokenSource = new CancellationTokenSource();
        var connected = await _serverConnectionHandler.TryConnectAsync(_cancellationTokenSource);
        if (!connected)
        {
            _contentDisplay.Content = _disconnectedView;
        }
        #endregion
        
        var applicationThread = new Thread(() =>
        {
            try
            {
                _app.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine("Something occured and the application crashed, disconnecting!");
                Console.WriteLine(e);
                _ = DisconnectAsync();
            }
        });
        applicationThread.Start();
    }

    private void OnLoginButtonClick(string username, string password)
    {
        var loginPacket = new LoginPacket(username, HashHelper.HashPassword(password));
        if (!_serverConnectionHandler.TrySendAsync(loginPacket).Result)
        {
            return;
        }

        var response = _serverConnectionHandler.GetResponseAsync(loginPacket.Id).Result;
        if (response is not { PacketType: PacketType.LoginInformation })
        {
            return;
        }

        // extract inner packet

        _contentDisplay.Content = _mainPageView;
    }

    private void OnSignUpButtonClick(string username, string password)
    {
        var hash = HashHelper.HashPassword(password);
        Console.WriteLine(hash);
        var loginPacket = new RegisterPacket(username, hash);
        if (!_serverConnectionHandler.TrySendAsync(loginPacket).Result)
        {
            return;
        }

        var response = _serverConnectionHandler.GetResponseAsync(loginPacket.Id).Result;
        if (response is not { PacketType: PacketType.LoginInformation })
        {
            return;
        }

        // extract inner packet

        _contentDisplay.Content = _mainPageView;
    }

    public async Task DisconnectAsync()
    {
        await _serverConnectionHandler.DisconnectAsync();
        _app.Kill();
        _session.Disconnect();
    }

    private void OnReconnectClick()
    {
        if (_serverConnectionHandler.Connected)
        {
            return;
        }
        
        _cancellationTokenSource = new CancellationTokenSource();
        if (_serverConnectionHandler.TryConnectAsync(_cancellationTokenSource).Result)
        {
            _contentDisplay.Content = _mainPageView;
        }
    }

    private void OnCreateLobbyClick()
    {
        var packet = new CreateLobbyPacket(AbstractPacket.NextPacketId());
        if (!_serverConnectionHandler.TrySendAsync(packet).Result)
        {
            return;
        }

        // var response = _serverConnectionHandler.GetResponseAsync(packet.Id).Result;
        // if (response is not { PacketType: PacketType.LobbyCreationInformation })
        // {
            // return;
        // }
        
        // Console.WriteLine(response);
        
        _contentDisplay.Content = new GameView();
        UpdateCtrlCKeyBind();
        Console.WriteLine("Create lobby button clicked!");
    }

    private void OnJoinLobbyClick()
    {
        _contentDisplay.Content = new JoinLobbyView(OnJoinButtonClick);
        UpdateCtrlCKeyBind();
        Console.WriteLine("Join lobby button clicked!");
    }
    
    private void OnJoinButtonClick(string code)
    {
        if (!_serverConnectionHandler.TrySendAsync(
                new JoinLobbyPacket(
                    int.Parse(code),
                    AbstractPacket.NextPacketId())).Result) // todo force int only
        {
            OnDisconnectedFromServer();
            return;
        }

        _contentDisplay.Content = new GameView();
        Console.WriteLine("Join button clicked!");
    }

    private void OnLeaderboardClick()
    {
        if (!_serverConnectionHandler.TrySendAsync(new ViewLeaderboardPacket(AbstractPacket.NextPacketId())).Result)
        {
            OnDisconnectedFromServer();
            return;
        }
        
        _contentDisplay.Content = new LeaderboardView();
        UpdateCtrlCKeyBind();
        Console.WriteLine("Leaderboard button clicked!");
    }

    private void OnDisconnectedFromServer()
    {
        _contentDisplay.Content = _disconnectedView;
        
        _app.RemoveGlobalBoundKey(CtrlCKey);
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { DisconnectAsync().Wait(); });
    }

    // a bunch of stupid hacks since there is no way of interacting with the main application
    // to change the displayed content nor is there a way to add a page specific key bind
    // and there is also no concept of a back stack, so its either create a new welcome window
    // or this hack
    private void UpdateCtrlCKeyBind()
    {
        _app.RemoveGlobalBoundKey(CtrlCKey);
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) =>
        {
            _contentDisplay.Content = _mainPageView;
            _app.RemoveGlobalBoundKey(CtrlCKey);
            _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { DisconnectAsync().Wait(); });
        });
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
    }
}