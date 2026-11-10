using Painter.Drawables;
using Painter.Drawing;
using Painter.Drawing.Canvas;

namespace Client.Game.Drawables;

public class TicTacToeBoardDrawable : Drawable
{
    private readonly Func<Button, short, short, Task> _onBoardButtonClickAsync;

    public TicTacToeBoardDrawable(Func<Button, short, short, Task> onBoardButtonClickAsync)
    {
        _onBoardButtonClickAsync = onBoardButtonClickAsync;
        
        BuildDrawable();
    }

    private void BuildDrawable()
    {
        var grid = new Grid
        {
            RowDefinitions = RowDefinition.Of("3, 1, 3, 1, 3"),
            ColumnDefinitions = ColumnDefinition.Of("7, 2, 7, 2, 7"),
        };

        for (var y = 0; y < 3; y++)
        {
            for (var x = 0; x < 3; x++)
            {
                var button = new Button
                {
                    UnfocusedColor = Defaults.ButtonUnfocusedColor,
                    FocusedColor = Defaults.ButtonFocusedColor,
                    DisabledColor = Defaults.ButtonDisabledColor
                };

                var indexX = (short) x;
                var indexY = (short) y;
                button.OnClickEvent += (_, _) => _onBoardButtonClickAsync(button, indexX, indexY);

                grid.SetContent(button, x * 2, y * 2);
            }
        }
        
        Attach(grid);
    }

    public override void Draw(ICanvas canvas)
    {
        if (canvas.Width < 25 || canvas.Height < 11)
        {
            canvas.Draw(0, canvas.Height / 2, "    Window too small    ", new StrokeStyle
            {
                BackgroundColor = Color.PaleVioletRed,
                ForegroundColor = Color.DarkRed
            });
            
            return;
        }
        
        base.Draw(canvas);
    }
}