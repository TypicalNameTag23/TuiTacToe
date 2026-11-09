using Painter.Drawables;
using Painter.Drawing;
using Painter.Drawing.Canvas;
using Painter.Events;
using Painter.Utilities;

namespace Client.Game.Drawables;

public class PasswordTextBox : LimitedTextBox
{
    public override void Draw(ICanvas canvas)
    {
        var width = Width;
        var height = Height;
        var text = Text;
        if (OverflowStyle == OverflowStyle.Ellipses)
        {
            var ellipsesStartIndex = width * height - 3;
            if (ellipsesStartIndex > 0 && text.Length > ellipsesStartIndex)
            {
                text = text[..ellipsesStartIndex] + "...";
            }
        }
        
        var lines = new string?[height];
        var lastLine = -1;
        var textIndex = 0;
        for (; lastLine + 1 < height && 
               textIndex < text.Length; 
             textIndex += width, lastLine++)
        {
            var endIndex = textIndex + width;
            lines[lastLine + 1] = text.SubstringSafely(textIndex, endIndex).Align(width, Alignment);
        }
        
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i] is null ? new string(' ', width) : lines[i]!;
            for (var j = 0; j < line.Length; j++)
            {
                var backgroundColor = Focused ? FocusedColor : BackgroundColor;
                var style = j + i * width == CursorPosition
                    ? new StrokeStyle
                    {
                        ForegroundColor = TextColor,
                        BackgroundColor = backgroundColor,
                        Underline = Focused
                    }
                    : new StrokeStyle
                    {
                        ForegroundColor = TextColor,
                        BackgroundColor = backgroundColor,
                        Underline = false
                    };

                var character = line[j] == ' ' ? ' ' : '*';
                canvas.Draw(X + j, Y + i, character, style);
            }
        }
    }
    
    public override void Interact(KeyPressedEventArgs args)
    {
        if (args.Character == ' ')
        {
            args.Handled = true;
            return;
        }
        
        base.Interact(args);
    }
}