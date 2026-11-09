using Client.Game.Drawables;
using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game.Views;

public class GameView : Drawable
{
    public GameView()
    {
        Attach(
            new Grid
                {
                    RowDefinitions = RowDefinition.Of("*, 1")
                }.SetContent(
                    new Grid
                    {
                        RowDefinitions = RowDefinition.Of("*, 11, *"),
                        ColumnDefinitions = ColumnDefinition.Of("*, 25, *")
                    }.SetContent(new TicTacToeBoardDrawable(), row: 1, column: 1))
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