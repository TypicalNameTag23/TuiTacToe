using Client.Game.Drawables;
using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game.Views;

public class GameView : Drawable
{
    private readonly Func<Button, short, short, Task> _onBoardButtonClickAsync;

    public GameView(Func<Button, short, short, Task> onBoardButtonClickAsync)
    {
        _onBoardButtonClickAsync = onBoardButtonClickAsync;
        BuildDrawable();
    }

    private void BuildDrawable()
    {
        Attach(
            new Grid { RowDefinitions = RowDefinition.Of("*, 1") }
                .SetContent(
                    new Grid
                    {
                        RowDefinitions = RowDefinition.Of("*, 11, *"),
                        ColumnDefinitions = ColumnDefinition.Of("*, 25, *")
                    }.SetContent(new TicTacToeBoardDrawable(_onBoardButtonClickAsync), row: 1, column: 1))
                .SetContent(
                    new TextBlock
                    {
                        Text = "    [TAB]: Cycle Focus  |  [SPACE]: Interact  |  [CTRL+C]: Exit Lobby    ",
                        OverflowStyle = OverflowStyle.Cutoff,
                        BackgroundColor = Color.Black
                    }, row: 1)
        );   
    }
}