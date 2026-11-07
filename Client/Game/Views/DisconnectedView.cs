using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game.Views;

public class DisconnectedView : Drawable
{
    public DisconnectedView(Action onReconnectButtonClick)
    {
        var reconnectButton = new Button
        {
            FocusedColor = Defaults.ButtonFocusedColor,
            Content = new TextBlock
            {
                Text = "Reconnect",
                Alignment = Alignment.Center,
                Y = 1
            }
        };
        
        reconnectButton.OnClick((_, _) => onReconnectButtonClick.Invoke());
        
        Attach(new Grid
            {
                RowDefinitions = RowDefinition.Of($"*, 1, {Defaults.SmallSpacing}, {Defaults.DefaultBlockSize}, *"),
            }
            .SetContent(
                new TextBlock
                {
                    Text = "Guh, looks like you disconnected from the server!",
                    Alignment = Alignment.Center
                }, row: 1)
            .SetContent(
                new Grid
                {
                    ColumnDefinitions = Defaults.Centered
                }
                .SetContent(reconnectButton, column: 1), row: 3)
        );
    }
}