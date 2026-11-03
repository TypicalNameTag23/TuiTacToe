using Painter;

namespace Client.Ssh.Parsing;

/// <summary>
/// Offers a way to statefully parse data received from an SSH client
/// </summary>
public interface IInputParser
{
    public KeyInfo? Parse(byte[] input);
}