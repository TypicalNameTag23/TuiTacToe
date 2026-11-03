using Client.Game.Drawables;
using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game.Views;

public class WelcomeView : Drawable
{
    public Action OnCreateLobbyClick { get; set; }
    public Action OnJoinLobbyClick { get; set; }
    public Action OnLeaderboardClick { get; set; }

    public WelcomeView()
    {
        var createLobbyButton = new Button
        {
            Height = 3,
            FocusedColor = new Color(53, 53, 53),
            Content = new TextBlock
            {
                Text = "Create Lobby",
                Alignment = Alignment.Center,
                Y = 1
            }
        };
        createLobbyButton.OnClickEvent += (_, _) => OnCreateLobbyClick();

        var joinLobbyButton = new Button
        {
            Height = 3,
            FocusedColor = new Color(53, 53, 53),
            Content = new TextBlock
            {
                Text = "Join Lobby",
                Alignment = Alignment.Center,
                Y = 1
            }
        };
        joinLobbyButton.OnClickEvent += (_, _) => OnJoinLobbyClick();

        var leaderboardButton = new Button
        {
            Height = 3,
            FocusedColor = new Color(53, 53, 53),
            Content = new TextBlock
            {
                Text = "Leaderboard",
                Alignment = Alignment.Center,
                Y = 1
            }
        };
        leaderboardButton.OnClickEvent += (_, _) => OnLeaderboardClick();
        
        Attach(
            new Grid
                {
                    RowDefinitions = RowDefinition.Of("*, 1")
                }
                .SetContent(
                    new Grid 
                        {
                            RowDefinitions = RowDefinition.Of("*, 3, 2, 3, 2, 3, 2, 3, *"),
                            ColumnDefinitions = ColumnDefinition.Of("*, 1.5*, *")
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
                                    Height = 1,
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