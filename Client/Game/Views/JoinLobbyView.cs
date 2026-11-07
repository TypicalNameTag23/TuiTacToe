using Client.Game.Drawables;
using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game.Views;

public class JoinLobbyView : Drawable
{
    private readonly Button _joinButton;
    private readonly TextBox _lobbyCodeTextBox;

    public JoinLobbyView(Action<string> joinButtonClick)
    {
        _lobbyCodeTextBox = new TextBox();
        _joinButton = new Button
        {
            FocusedColor = Defaults.ButtonFocusedColor,
            Content = new TextBlock
            {
                Text = "Join",
                Alignment = Alignment.Center
            }
        };
        _joinButton.OnClickEvent += (_, _) => joinButtonClick(_lobbyCodeTextBox.Text);

        BuildDrawable();
    }

    private void BuildDrawable()
    {
        Attach(
            new Grid
                {
                    RowDefinitions = RowDefinition.Of("*, 1")
                }
                .SetContent(
                    new Grid
                        {
                            RowDefinitions = RowDefinition.Of(
                                $"*, {Defaults.DefaultBlockSize}, {Defaults.DefaultSpacing}, {Defaults.SmallBlockSize}, *"),
                            ColumnDefinitions = Defaults.Centered
                        }
                        .SetContent(new Panel
                        {
                            Height = 3,
                            Content =
                            [
                                new HsvColorSpaceVisualizer
                                {
                                    SaturationOffset = 0.5,
                                    Height = 15,
                                    Width = 350,
                                    Value = 0.7,
                                    Focusable = false
                                },
                                new TextBlock
                                {
                                    Text = "Enter Lobby Code",
                                    Alignment = Alignment.Center,
                                    Height = 1,
                                    Y = 1
                                }
                            ]
                        }, row: 1, column: 1)
                        .SetContent(
                            new Grid
                                {
                                    ColumnDefinitions = ColumnDefinition.Of("*, 1, 8")
                                }
                                .SetContent(_lobbyCodeTextBox, column: 0)
                                .SetContent(_joinButton, column: 2), row: 3, column: 1), row: 0)
                .SetContent(
                    new TextBlock
                    {
                        Text = "    [TAB]: Cycle Focus  |  [SPACE]: Interact  |  [CTRL+C]: Previous Page    ",
                        OverflowStyle = OverflowStyle.Cutoff,
                        BackgroundColor = Color.Black
                    }, row: 1)
        );
    }
}