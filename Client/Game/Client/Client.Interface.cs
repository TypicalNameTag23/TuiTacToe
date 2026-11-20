using Client.Game.Drawables;
using Client.Game.Views;
using Common.Packets;
using Common.Packets.Handshake;
using Common.Packets.Leaderboard;
using Painter;
using Painter.Drawables;

namespace Client.Game;

// Interface actions are held here
public partial class Client
{
    private static readonly Key CtrlCKey = new(ConsoleKey.C, ConsoleModifiers.Control);
    private ContentControl _contentDisplay;
    
    private async Task OnLoginButtonClickAsync(string username, string password)
    {
        if (_contentDisplay.Content is not LoginView view)
        {
            return;
        }
        
        view.DisableButtons();
        var loginPacket = new LoginPacket(username, password);
        var response = await _serverConnectionHandler.TrySendAndGetResponseAsync(loginPacket);
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
        
        var loginPacket = new RegisterPacket(username, password);
        var response = await _serverConnectionHandler.TrySendAndGetResponseAsync(loginPacket);
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
        await ConnectToBackendAsync();
        view.EnableButtons();
    }

    private async Task OnCreateLobbyClickAsync()
    {
        if (_gameSessionManager.InGame || _contentDisplay.Content is not MainPageView view)
        {
            return;
        }

        view.DisableButtons();
        if (!await _gameSessionManager.TryCreateGameLobby())
        {
            view.EnableButtons();
            return;
        }
        
        _app.QueueAction(() => _contentDisplay.Content = new GameView(_gameSessionManager, _app));
        _app.RemoveGlobalBoundKey(CtrlCKey);
        _app.AddGlobalKeyBind(CtrlCKey, (_, _) => { _ = LeaveLobbyAsync(); });
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
        if (!await _gameSessionManager.TryJoinLobby(code))
        {
            view.EnableButtons();
            return;
        }
        
        _app.QueueAction(() => _contentDisplay.Content = new GameView(_gameSessionManager, _app));
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

    private Drawable BuildApplicationContent()
    {
        return new Grid { RowDefinitions = RowDefinition.Of("1, *") }
            .SetContent(new FpsTacker())
            .SetContent(_contentDisplay, row: 1);
    }
}