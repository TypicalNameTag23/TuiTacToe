using Painter.Drawables;
using Painter.Events;

namespace Client.Game.Drawables;

public class NumericalTextBox : LimitedTextBox
{
    public override void Interact(KeyPressedEventArgs args)
    {
        if (args.Character > 31 && args.Character < 127 && (Text.Length >= CharacterLimit || args.Character < 48 || args.Character > 57))
        {
            args.Handled = true;
            return;
        }

        base.Interact(args);
    }
}