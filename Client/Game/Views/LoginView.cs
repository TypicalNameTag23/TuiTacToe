using Client.Game.Drawables;
using Painter.Drawables;
using Painter.Drawing;

namespace Client.Game.Views;

public class LoginView : Drawable
{
    private readonly TextBox _usernameTextBox;
    private readonly TextBox _passwordTextBox;
    private Button _loginButton;
    private Button _signUpButton;

    public LoginView(
        Action<string, string> loginButtonClick,
        Action<string, string> signUpButtonClick)
    {
        _usernameTextBox = new LimitedTextBox  { CharacterLimit = 32 };
        _passwordTextBox = new PasswordTextBox { CharacterLimit = 32 };
        
        _loginButton = new Button
        {
            FocusedColor = Defaults.ButtonFocusedColor,
            Content = new TextBlock
            {
                Text = "Login",
                Alignment = Alignment.Center
            }
        };
        _loginButton.OnClickEvent += (_, _) => loginButtonClick(_usernameTextBox.Text, _passwordTextBox.Text);
        
        _signUpButton = new Button
        {
            FocusedColor = Defaults.ButtonFocusedColor,
            Content = new TextBlock
            {
                Text = "Sign Up",
                Alignment = Alignment.Center
            }
        };
        _signUpButton.OnClickEvent += (_, _) => signUpButtonClick(_usernameTextBox.Text, _passwordTextBox.Text);
        
        BuildDrawable();
    }

    private void BuildDrawable()
    {
        var buttonGrid = new Grid
            {
                ColumnDefinitions = ColumnDefinition.Of($"*, {Defaults.DefaultSpacing}, *")
            }
            .SetContent(_loginButton, column: 0)
            .SetContent(_signUpButton, column: 2);

        var grid = new Grid
            {
                RowDefinitions = RowDefinition.Of(
                    $"*, " +
                    $"{Defaults.SmallBlockSize}, {Defaults.SmallBlockSize}, {Defaults.SmallSpacing}, " +
                    $"{Defaults.SmallBlockSize}, {Defaults.SmallBlockSize}, {Defaults.SmallSpacing}, " +
                    $"{Defaults.SmallBlockSize}, " +
                    $"*"),
                ColumnDefinitions = Defaults.Centered
            }
            .SetContent(new TextBlock { Text = "Username" }, row: 1, column: 1)
            .SetContent(_usernameTextBox, row: 2, column: 1)
            .SetContent(new TextBlock { Text = "Password" }, row: 4, column: 1)
            .SetContent(_passwordTextBox, row: 5, column: 1)
            .SetContent(buttonGrid, row: 7, column: 1);
        
        Attach(grid);
    }
}