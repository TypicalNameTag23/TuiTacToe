using Client.Game.Drawables;
using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game.Views;

public class MainPageView : Drawable
{
    private readonly Button _createLobbyButton;
    private readonly Button _joinLobbyButton;
    private readonly Button _leaderboardButton;

    public MainPageView(Func<Task> onCreateLobbyClickAsync, Action onJoinLobbyClick, Func<Task> onLeaderboardClickAsync)
    {
        _createLobbyButton = new Button
        {
            FocusedColor = Defaults.ButtonFocusedColor,
            UnfocusedColor = Defaults.ButtonUnfocusedColor,
            DisabledColor = Defaults.ButtonDisabledColor,
            Content = new TextBlock
            {
                Text = "Create Lobby",
                Alignment = Alignment.Center,
                Y = 1
            }
        };

        _joinLobbyButton = new Button
        {
            FocusedColor = Defaults.ButtonFocusedColor,
            UnfocusedColor = Defaults.ButtonUnfocusedColor,
            DisabledColor = Defaults.ButtonDisabledColor,
            Content = new TextBlock
            {
                Text = "Join Lobby",
                Alignment = Alignment.Center,
                Y = 1
            }
        };

        _leaderboardButton = new Button
        {
            FocusedColor = Defaults.ButtonFocusedColor,
            UnfocusedColor = Defaults.ButtonUnfocusedColor,
            DisabledColor = Defaults.ButtonDisabledColor,
            Content = new TextBlock
            {
                Text = "Leaderboard",
                Alignment = Alignment.Center,
                Y = 1
            }
        };
        
        _createLobbyButton.OnClickEvent += (_, _) => onCreateLobbyClickAsync();
        _joinLobbyButton.OnClickEvent += (_, _) => onJoinLobbyClick();
        _leaderboardButton.OnClickEvent += (_, _) => onLeaderboardClickAsync();

        BuildDrawable();
    }

    public void EnableButtons()
    {
        _createLobbyButton.Focusable = true;
        _joinLobbyButton.Focusable = true;
        _leaderboardButton.Focusable = true;
    }

    public void DisableButtons()
    {
        _createLobbyButton.Focusable = false;
        _joinLobbyButton.Focusable = false;
        _leaderboardButton.Focusable = false;
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
                            RowDefinitions = RowDefinition.Of("*, 3, 2, 3, 2, 3, 2, 3, *"),
                            ColumnDefinitions = Defaults.Centered
                        }
                        .SetContent(new Panel
                        {
                            Height = 3,
                            Content = [
                                new HsvColorSpaceVisualizer
                                {
                                    SaturationOffset = 0.5,
                                    Height = 400,
                                    Width = 500,
                                    Value = 0.5,
                                    Focusable = true
                                },
                                new TextBlock
                                {
                                    Text = "TUI-Tac-Toe",
                                    Alignment = Alignment.Center,
                                    Y = 1,
                                }
                            ]
                        }, row: 1, column: 1)
                        .SetContent(_createLobbyButton, row: 3, column: 1)
                        .SetContent(_joinLobbyButton, row: 5, column: 1)
                        .SetContent(_leaderboardButton, row: 7, column: 1), row: 0)
                .SetContent(new TextBlock
                {
                    Text = "    [TAB]: Cycle Focus  |  [SPACE]: Interact  |  [CTRL+C]: Quit    ",
                    OverflowStyle = OverflowStyle.Cutoff,
                    BackgroundColor = Color.Black
                }, row: 1)
        );
    }
}