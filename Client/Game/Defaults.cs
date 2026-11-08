using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game;

public static class Defaults
{
    public static readonly Color ButtonFocusedColor = new(53, 53, 53);
    public static readonly Color ButtonUnfocusedColor = Color.Black;
    public static readonly Color ButtonDisabledColor = new(93, 93, 93);
    
    public const int DefaultBlockSize = 3;
    public const int SmallBlockSize = 1;
    public const int DefaultSpacing = 2;
    public const int SmallSpacing = 1;
    
    public static readonly IReadOnlyList<ColumnDefinition> Centered = ColumnDefinition.Of("*, 1.5*, *");
}