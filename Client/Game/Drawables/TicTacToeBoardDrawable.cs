using Painter.Drawables;
using Painter.Drawing;
using Painter.Drawing.Canvas;

namespace Client.Game.Drawables;

public class TicTacToeBoardDrawable : Drawable
{
    public TicTacToeBoardDrawable()
    {
        Attach(
            new Grid
            {
                RowDefinitions = RowDefinition.Of("3, 1, 3, 1, 3"),
                ColumnDefinitions = ColumnDefinition.Of("7, 2, 7, 2, 7"),
            }
            .SetContent(new Button(), row: 0, column: 0)
            .SetContent(new Button(), row: 0, column: 2)
            .SetContent(new Button(), row: 0, column: 4)
            .SetContent(new Button(), row: 2, column: 0)
            .SetContent(new Button(), row: 2, column: 2)
            .SetContent(new Button(), row: 2, column: 4)
            .SetContent(new Button(), row: 4, column: 0)
            .SetContent(new Button(), row: 4, column: 2)
            .SetContent(new Button(), row: 4, column: 4));
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