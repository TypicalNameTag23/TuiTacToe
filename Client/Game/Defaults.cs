using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game;

public static class Defaults
{
    public static Color ButtonFocusedColor = new Color(53, 53, 53);
    
    public static int DefaultBlockSize = 3;
    public static int SmallBlockSize = 1;
    public static int DefaultSpacing = 2;
    public static int SmallSpacing = 1;
    
    public static ColumnDefinition[] Centered = ColumnDefinition.Of("*, 1.5*, *");
}