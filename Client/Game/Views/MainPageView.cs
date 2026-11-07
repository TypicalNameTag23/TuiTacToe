using Client.Game.Drawables;
using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game.Views;

public class MainPageView : Drawable
{
    public MainPageView(
        Action onCreateLobbyClick,
        Action onJoinLobbyClick,
        Action onLeaderboardClick)
    {
        var createLobbyButton = new Button
        {
            FocusedColor = Defaults.ButtonFocusedColor,
            Content = new TextBlock
            {
                Text = "Create Lobby",
                Alignment = Alignment.Center,
                Y = 1
            }
        };
        createLobbyButton.OnClickEvent += (_, _) => onCreateLobbyClick();

        var joinLobbyButton = new Button
        {
            FocusedColor = Defaults.ButtonFocusedColor,
            Content = new TextBlock
            {
                Text = "Join Lobby",
                Alignment = Alignment.Center,
                Y = 1
            }
        };
        joinLobbyButton.OnClickEvent += (_, _) => onJoinLobbyClick();

        var leaderboardButton = new Button
        {
            FocusedColor = Defaults.ButtonFocusedColor,
            Content = new TextBlock
            {
                Text = "Leaderboard",
                Alignment = Alignment.Center,
                Y = 1
            }
        };
        leaderboardButton.OnClickEvent += (_, _) => onLeaderboardClick();
        
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
                        .SetContent(createLobbyButton, row: 3, column: 1)
                        .SetContent(joinLobbyButton, row: 5, column: 1)
                        .SetContent(leaderboardButton, row: 7, column: 1), row: 0)
                .SetContent(new TextBlock
                {
                    Text = "    [TAB]: Cycle Focus  |  [SPACE]: Interact  |  [CTRL+C]: Quit    ",
                    OverflowStyle = OverflowStyle.Cutoff,
                    BackgroundColor = Color.Black
                }, row: 1)
        );
    }
    
}