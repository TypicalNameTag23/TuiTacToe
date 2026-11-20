using System.Net;
using Client.Game.Views;
using Painter;
using Painter.Drawables;
using Painter.Platform;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

namespace Client.Game;

public partial class Client(Terminal terminal, IPAddress serverAddress, ushort serverPort) : IDisposable
{
    private Application _app;

    private CancellationTokenSource _cancellationTokenSource;
    private ServerConnectionHandler _serverConnectionHandler;
    private GameSessionManager _gameSessionManager;

    public EventHandler<EventArgs>? OnDisconnect;
    
    public async Task StartAsync()
    {
        _contentDisplay = new ContentControl();
        _app = new Application(terminal);
        _app.Content = BuildApplicationContent();
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { DisconnectAsync().Wait(); });

        _serverConnectionHandler = new ServerConnectionHandler(serverAddress, serverPort);
        _serverConnectionHandler.OnConnectionClosed += OnDisconnectedFromBackend;
        await ConnectToBackendAsync();

        _gameSessionManager = new GameSessionManager(_serverConnectionHandler);
        _gameSessionManager.OnGameCompleted += OnGameCompleted;
        
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

    public async Task DisconnectAsync()
    {
        OnDisconnect?.Invoke(this, EventArgs.Empty);
        await _cancellationTokenSource.CancelAsync();
        await _serverConnectionHandler.DisconnectAsync();
        
        _app.Kill();
    }

    public void Dispose()
    {
        _cancellationTokenSource.Dispose();
    }

    private void OnGameCompleted(object? sender, bool e)
    {
        _app.RemoveGlobalBoundKey(CtrlCKey);
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) =>
        {
            _app.RemoveGlobalBoundKey(CtrlCKey);
            _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { DisconnectAsync().Wait(); });
            _app.QueueAction(() => _contentDisplay.Content = new MainPageView(OnCreateLobbyClickAsync, OnJoinLobbyClick, OnLeaderboardClickAsync));
        });
    }

    private async Task LeaveLobbyAsync()
    {
        if (!_gameSessionManager.InGame)
        {
            return;
        }
        
        _app.RemoveGlobalBoundKey(CtrlCKey);
        if (!await _gameSessionManager.TryLeaveGameLobby())
        {
            _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { _ = LeaveLobbyAsync(); });
            return;
        }

        _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { DisconnectAsync().Wait(); });
        _app.QueueAction(() => _contentDisplay.Content = new MainPageView(OnCreateLobbyClickAsync, OnJoinLobbyClick, OnLeaderboardClickAsync));
    }

    private async Task ConnectToBackendAsync()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        
        if (await _serverConnectionHandler.TryConnectAsync(_cancellationTokenSource))
        {
            _app.QueueAction(() => _contentDisplay.Content = new LoginView(OnLoginButtonClickAsync, OnSignUpButtonClickAsync));
        }
    }

    private void OnDisconnectedFromBackend(object? s, EventArgs e)
    {
        _app.RemoveGlobalBoundKey(CtrlCKey);
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { DisconnectAsync().Wait(); });
        
        if (_contentDisplay.Content is DisconnectedView)
        {
            return;
        }
        
        _app.QueueAction(() => { _contentDisplay.Content = new DisconnectedView(OnReconnectClickAsync); });
    }
}