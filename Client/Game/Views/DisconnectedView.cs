using Painter.Drawables;
using Painter.Drawing;

namespace SshThing.Views;

public class DisconnectedView : Drawable
{
    public Action OnReconnectButtonClick { get; set; }

    public DisconnectedView()
    {
        var reconnectButton = new Button
        {
            Content = new TextBlock { Text = "Reconnect" }
        };
        
        reconnectButton.OnClick((_, _) => OnReconnectButtonClick!.Invoke());
        
        Attach(new Grid
            {
                RowDefinitions = RowDefinition.Of("*, 1, 1, 3, *"),
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
                    ColumnDefinitions = ColumnDefinition.Of("*, 1.5*, *"),
                }
                .SetContent(reconnectButton, column: 1), row: 3)
        );
    }
}