using Client.Game.Drawables;
using Common;
using Painter;
using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game.Views;

public class GameView : Drawable
{
    private readonly GameSessionManager _gameSessionManager;
    private readonly Application _app; // hack to schedule ui updates on ui thread since im lazy and dont feel like coming up with a proper system for painter rn
    private TextBlock _textBlock;
    private TicTacToeBoardDrawable _ticTacToeBoard;

    public GameView(GameSessionManager gameSessionManager, Application app)
    {
        _gameSessionManager = gameSessionManager;
        _app = app;
        _gameSessionManager.OnOpponentJoin += OnOpponentJoin;
        _gameSessionManager.OnOpponentDisconnect += OnOpponentDisconnect;
        _gameSessionManager.OnBoardUpdateReceived += OnBoardUpdateReceived;
        _gameSessionManager.OnGameCompleted += OnGameCompleted;
        
        BuildDrawable();
    }

    private void OnGameCompleted(object? sender, bool e)
    {
        _textBlock.Text = e ? "You wonnn!!!!!!!!!!!" : "You lost :(";
    }

    private void OnBoardUpdateReceived(object? sender, (short, short) e)
    {
        if (!_gameSessionManager.InGame)
        {
            return;
        }
        
        var playerCharacter = _gameSessionManager.PlayerType == PlayerType.X ? "X" : "O";
        _textBlock.Text = $"You: {playerCharacter} | Opponent: {_gameSessionManager.OpponentUsername} | YOUR TURN";
        _app.QueueAction(() => _ticTacToeBoard.GetButton(e.Item1, e.Item2).Content = new TextBlock
            { Text = _gameSessionManager.PlayerType == PlayerType.X ? "O" : "X" });
    }

    private void OnOpponentDisconnect(object? sender, EventArgs e)
    {
        if (!_gameSessionManager.InGame)
        {
            return;
        }
        
        _textBlock.Text = "Opponent quit, you wonnn!!!!!!!!!!!";
    }

    private void OnOpponentJoin(object? sender, string e)
    {
        var playerCharacter = _gameSessionManager.PlayerType == PlayerType.X ? "X" : "O";
        _textBlock.Text = $"You: {playerCharacter} | Opponent: {e} | YOUR TURN";
    }

    private void BuildDrawable()
    {
        _ticTacToeBoard = new TicTacToeBoardDrawable(OnBoardButtonClickAsync);
        var playerCharacter = _gameSessionManager.PlayerType == PlayerType.X ? "X" : "O";
        _textBlock = new TextBlock
        {
            Text = _gameSessionManager.OpponentUsername == null ? $"You: {playerCharacter} | Lobby Code: {_gameSessionManager.LobbyCode}" : $"You: {playerCharacter} | Opponent: {_gameSessionManager.OpponentUsername} | OPPONENTS TURN",
            TextColor = Color.Coral,
            BackgroundColor = Color.Black,
            Width = 100
        };
        
        Attach(
            new Grid { RowDefinitions = RowDefinition.Of("1, *, 1") }
                .SetContent(_textBlock, 0)
                .SetContent(
                    new Grid
                    {
                        RowDefinitions = RowDefinition.Of("*, 11, *"),
                        ColumnDefinitions = ColumnDefinition.Of("*, 25, *")
                    }.SetContent(_ticTacToeBoard, row: 1, column: 1), row: 1)
                .SetContent(
                    new TextBlock
                    {
                        Text = "    [TAB]: Cycle Focus  |  [SPACE]: Interact  |  [CTRL+C]: Exit Lobby    ",
                        OverflowStyle = OverflowStyle.Cutoff,
                        BackgroundColor = Color.Black
                    }, row: 2)
        );   
    }

    private async Task OnBoardButtonClickAsync(Button button, short x, short y)
    {
        if (!_gameSessionManager.InGame)
        {
            return;
        }
        
        if (!await _gameSessionManager.TryPlayingPosition(x, y))
        {
            return;
        }
        
        _app.QueueAction(() => button.Content = new TextBlock { Text = _gameSessionManager.PlayerType == PlayerType.X ? "X" : "O" });
        var playerCharacter = _gameSessionManager.PlayerType == PlayerType.X ? "X" : "O";

        if (_gameSessionManager.InGame)
        {
            _textBlock.Text = $"You: {playerCharacter} | Opponent: {_gameSessionManager.OpponentUsername} | OPPONENTS TURN";
        }
    }
}