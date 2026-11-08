using System.Net.Http.Headers;
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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Client.Game;

public class Client(PtyArgs args, Session session) : IDisposable
{
    private static readonly Key CtrlCKey = new(ConsoleKey.C, ConsoleModifiers.Control);
    private const string BackendServerAddress = "127.0.0.1";
    private const int BackendServerPort = 4343;

    private ContentControl _contentDisplay;
    private GameSession? _gameSession;
    private Application _app;

    private CancellationTokenSource _cancellationTokenSource;
    private ServerConnectionHandler _serverConnectionHandler;

    public async Task StartAsync()
    {
        _contentDisplay = new ContentControl();
        
        var terminal = new SshTerminal(args, new XTermInputParser());
        _app = new Application(terminal)
        {
            Content = new Grid { RowDefinitions = RowDefinition.Of("1, *") }
                .SetContent(new FpsTacker())
                .SetContent(_contentDisplay, row: 1)
        };
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { DisconnectAsync().Wait(); });

        _serverConnectionHandler = new ServerConnectionHandler(BackendServerAddress, BackendServerPort);
        _serverConnectionHandler.OnConnectionClosed += OnDisconnectedFromServer;
        _serverConnectionHandler.OnPacketReceived += OnPacketReceived;
        await ConnectAsync();
        
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

    // like yeah idk this code is shit but whatever
    private void OnPacketReceived(object? _, AbstractPacket packet)
    {
        if (packet is OpponentConnectedPacket opponentConnectedPacket)
        {
            
        }
        if (packet is OpponentDisconnectedPacket opponentDisconnectedPacket)
        {
            
        }
    }

    public async Task DisconnectAsync()
    {
        await _serverConnectionHandler.DisconnectAsync();
        
        _app.Kill();
        session.Disconnect();
    }

    private async Task OnLoginButtonClickAsync(string username, string password)
    {
        if (_contentDisplay.Content is not LoginView view)
        {
            return;
        }
        
        view.DisableButtons();
        var loginPacket = new LoginPacket(username, HashHelper.HashPassword(password));
        if (!await _serverConnectionHandler.TrySendAsync(loginPacket))
        {
            return;
        }

        var response = await _serverConnectionHandler.GetResponseAsync(loginPacket.Id);
        if (response is not { PacketType: PacketType.Accepted })
        {
            view.EnableButtons();
            return;
        }
        
        _app.QueueAction(() => _contentDisplay.Content = new MainPageView(OnCreateLobbyClickAsync, OnJoinLobbyClick, OnLeaderboardClickAsync));
    }

    private async Task OnSignUpButtonClickAsync(string username, string password)
    {
        if (_contentDisplay.Content is not LoginView view)
        {
            return;
        }
        
        var loginPacket = new RegisterPacket(username, HashHelper.HashPassword(password));
        if (!await _serverConnectionHandler.TrySendAsync(loginPacket))
        {
            return;
        }

        var response = await _serverConnectionHandler.GetResponseAsync(loginPacket.Id);
        if (response is not { PacketType: PacketType.Accepted })
        {
            view.EnableButtons();
            return;
        }
        
        _app.QueueAction(() => _contentDisplay.Content = new MainPageView(OnCreateLobbyClickAsync, OnJoinLobbyClick, OnLeaderboardClickAsync));
    }

    private async Task OnReconnectClickAsync()
    {
        if (_contentDisplay.Content is not DisconnectedView view || _serverConnectionHandler.Connected)
        {
            return;
        }

        view.DisableButtons();
        await ConnectAsync();
        view.EnableButtons();
    }

    private async Task ConnectAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        if (await _serverConnectionHandler.TryConnectAsync(_cancellationTokenSource))
        {
            _app.QueueAction(() => _contentDisplay.Content = new LoginView(OnLoginButtonClickAsync, OnSignUpButtonClickAsync));
        }
    }

    private async Task OnCreateLobbyClickAsync()
    {
        if (_contentDisplay.Content is not MainPageView view)
        {
            return;
        }

        view.DisableButtons();
        var packet = new CreateLobbyPacket();
        if (!_serverConnectionHandler.TrySendAsync(packet).Result)
        {
            return;
        }
        
        var response = await _serverConnectionHandler.GetResponseAsync(packet);
        if (response is not { PacketType: PacketType.LobbyCreationInformation })
        {
            view.EnableButtons();
            return;
        }
        
        _app.QueueAction(() => _contentDisplay.Content = new GameView());
        _app.RemoveGlobalBoundKey(CtrlCKey);
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { _ = LeaveLobbyAsync(); });
    }

    private async Task LeaveLobbyAsync()
    {
        _app.RemoveGlobalBoundKey(CtrlCKey);
        var packet = new LeaveLobbyPacket();
        if (!await _serverConnectionHandler.TrySendAsync(packet))
        {
            _gameSession = null;
            return;
        }

        var response = await _serverConnectionHandler.GetResponseAsync(packet.Id);
        if (response is not { PacketType: PacketType.Accepted })
        {
            _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { _ = LeaveLobbyAsync(); });
            return;
        }

        _gameSession = null;
        _app.RemoveGlobalBoundKey(CtrlCKey);
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { DisconnectAsync().Wait(); });
        _app.QueueAction(() => _contentDisplay.Content = new MainPageView(OnCreateLobbyClickAsync, OnJoinLobbyClick, OnLeaderboardClickAsync));
    }

    private void OnJoinLobbyClick()
    {
        if (_contentDisplay.Content is not MainPageView)
        {
            return;
        }
        
        _contentDisplay.Content = new JoinLobbyView(OnJoinButtonClickAsync);
        UpdateCtrlCCloseKeyBindToBackKeyBind();
    }
    
    private async Task OnJoinButtonClickAsync(int code)
    {
        if (_contentDisplay.Content is not JoinLobbyView view)
        {
            return;
        }

        view.DisableButtons();
        var packet = new JoinLobbyPacket(code);
        if (!await _serverConnectionHandler.TrySendAsync(packet)) 
        {
            return;
        }

        var response = await _serverConnectionHandler.GetResponseAsync(packet);
        if (response is not { PacketType: PacketType.LobbyJoinInformation })
        {
            view.EnableButtons();
            return;
        }

        _app.QueueAction(() => _contentDisplay.Content = new GameView());
        _app.RemoveGlobalBoundKey(CtrlCKey);
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { _ = LeaveLobbyAsync(); });
    }

    private async Task OnLeaderboardClickAsync()
    {
        if (!await _serverConnectionHandler.TrySendAsync(new ViewLeaderboardPacket()))
        {
            return;
        }
        
        _app.QueueAction(() => _contentDisplay.Content = new LeaderboardView());
        UpdateCtrlCCloseKeyBindToBackKeyBind();
    }

    private void OnDisconnectedFromServer(object? s, EventArgs e)
    {
        _app.RemoveGlobalBoundKey(CtrlCKey);
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { DisconnectAsync().Wait(); });
        
        if (_contentDisplay.Content is DisconnectedView)
        {
            return;
        }
        
        _app.QueueAction(() => { _contentDisplay.Content = new DisconnectedView(OnReconnectClickAsync); });
    }

    private void UpdateCtrlCCloseKeyBindToBackKeyBind()
    {
        _app.RemoveGlobalBoundKey(CtrlCKey);
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) =>
        {
            _app.QueueAction(() => _contentDisplay.Content = new MainPageView(OnCreateLobbyClickAsync, OnJoinLobbyClick, OnLeaderboardClickAsync));
            _app.RemoveGlobalBoundKey(CtrlCKey);
            _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { DisconnectAsync().Wait(); });
        });
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
    }
}