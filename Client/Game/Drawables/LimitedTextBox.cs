using Painter.Drawables;
using Painter.Events;

namespace Client.Game.Drawables;

/// <summary>
/// Extension of <see cref="TextBox"/> that accepts a limited number of characters
/// </summary>
public class LimitedTextBox : TextBox
{
    public uint CharacterLimit { get; set; }

    public override void Interact(KeyPressedEventArgs args)
    {
        if (args.Character > 31 && args.Character < 127 && Text.Length >= CharacterLimit)
        {
            args.Handled = true;
            return;
        }
        
        base.Interact(args);
    }
}