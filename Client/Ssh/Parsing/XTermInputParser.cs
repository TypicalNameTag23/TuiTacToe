using System.Text;
using Painter;

namespace Client.Ssh.Parsing;

/// <summary>
/// Basic input parser for inputs received from an X-term based terminal
/// </summary>
public class XTermInputParser : IInputParser
{
    private const byte LowerCharacterBound = 0x20;
    private const byte UpperCharacterBound = 0x7E;
    private const byte LowerLowercaseLetterBound = 0x61;
    private const byte UpperLowercaseLetterBound = 0x7A;
    // private const byte Backspace = 0x08;
    private const byte Tab = 0x09;
    private const byte Backspace = 0x7F; // idk why but ascii character sheets list Backspace as 0x08 and delete as 0x7F... || never mind 0x08 is used for ctrl+delete
    private const byte Enter = 0x0D;
    private const byte CtrlC = 0x03;

    // todo: create own mapping of characters and defer to that
    public KeyInfo? Parse(byte[] input)
    {
        switch (input)
        {
            case [>= LowerCharacterBound and <= UpperCharacterBound]:
                var consoleKey = (ConsoleKey) input[0];
                if (input is [>= LowerLowercaseLetterBound and <= UpperLowercaseLetterBound])
                {
                    // hackity hack
                    consoleKey = (ConsoleKey) (char) (input[0] - 32);
                }
                
                Console.WriteLine($"Received 0x{BitConverter.ToString(input)}: {Encoding.ASCII.GetString(input)}; {consoleKey}");
                return new KeyInfo(new Key(consoleKey), (char) input[0]);
            case [Tab]:
                Console.WriteLine($"Received 0x{BitConverter.ToString(input)}: Tab");
                return new KeyInfo(new Key(ConsoleKey.Tab), null);
            case [Backspace]:
                Console.WriteLine($"Received 0x{BitConverter.ToString(input)}: Backspace");
                return new KeyInfo(new Key(ConsoleKey.Backspace), null);
            case [Enter]:
                Console.WriteLine($"Received 0x{BitConverter.ToString(input)}: Enter");
                return new KeyInfo(new Key(ConsoleKey.Enter), null);
            case [CtrlC]:
                Console.WriteLine($"Received 0x{BitConverter.ToString(input)}: Ctrl+C");
                return new KeyInfo(new Key(ConsoleKey.C, ConsoleModifiers.Control), null);
            default:
                Console.WriteLine($"Received unknown input 0x{BitConverter.ToString(input)}, ignored.");
                return null;
        }
    }
}