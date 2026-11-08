using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game.Views;

public class DisconnectedView : Drawable
{
    private readonly Button _reconnectButton;

    public DisconnectedView(Func<Task> onReconnectButtonClick)
    {
        _reconnectButton = new Button
        {
            FocusedColor = Defaults.ButtonFocusedColor,
            UnfocusedColor = Defaults.ButtonUnfocusedColor,
            DisabledColor = Defaults.ButtonDisabledColor,
            Content = new TextBlock
            {
                Text = "Reconnect",
                Alignment = Alignment.Center,
                Y = 1
            }
        };
        
        _reconnectButton.OnClick((_, _) => onReconnectButtonClick.Invoke());
        BuildDrawable();
    }

    public void EnableButtons()
    {
        _reconnectButton.Focusable = true;
    }
    
    public void DisableButtons()
    {
        _reconnectButton.Focusable = false;
    }
    
    private void BuildDrawable()
    {
        Attach(new Grid { RowDefinitions = RowDefinition.Of($"*, 1, {Defaults.SmallSpacing}, {Defaults.DefaultBlockSize}, *"), }
            .SetContent(
                new TextBlock
                {
                    Text = "Guh, looks like you disconnected from the server!",
                    Alignment = Alignment.Center
                }, row: 1)
            .SetContent(
                new Grid { ColumnDefinitions = Defaults.Centered }
                    .SetContent(_reconnectButton, column: 1), row: 3)
        );
    }
}