using System.Text;
using Client.Ssh.Parsing;
using FxSsh.Services;
using Painter;
using Painter.Events;
using Painter.Platform;

namespace Client.Ssh;

/// <summary>
/// Terminal implementation for a terminal connected over SSH
/// </summary>
public class SshTerminal : Terminal
{
    private readonly SessionChannel _channel;
    private readonly Queue<KeyInfo> _keyBuffer = [];
    private int _width;
    private int _height;

    public SshTerminal(PtyArgs ptyArgs, IInputParser inputParser)
    {
        _channel = ptyArgs.Channel;
        _channel.WindowChange += (_, windowChangeArgs) =>
        {
            _width = (int) windowChangeArgs.WidthColumns;
            _height = (int) windowChangeArgs.HeightRows;
        };
        
        _channel.DataReceived += (_, dataArgs) =>
        {
            var key = inputParser.Parse(dataArgs.ToArray());
            if (key == null)
            {
                return;
            }
            
            // Console.WriteLine($"Added key {key.Key.KeyType} to buffer. {key}");
            _keyBuffer.Enqueue(key);
        };
        
        _width = (int) ptyArgs.WidthChars;
        _height = (int) ptyArgs.HeightRows;
    }
    
    public override void Write(string text)
    {
        _channel.SendData(Encoding.UTF8.GetBytes(text));
    }

    public override KeyInfo? ReadKey()
    {
        if (_keyBuffer.Count == 0)
        {
            return null;
        }
        
        var key = _keyBuffer.Dequeue();
        // Console.WriteLine($"Popped key {key.Key.KeyType} from buffer. {key}");

        return key;
    }

    public override void Flush()
    {
        BufferStyleReset();
        _channel.SendData(Encoding.UTF8.GetBytes(Buffer.ToString()));
        Buffer.Clear();
    }

    public override void PollSizeChange()
    {
        if (Width == _width && Height == _height)
        {
            return;
        }

        Width = _width;
        Height = _height;
        OnTerminalSizeChanged(this, new TerminalChangedSizeEventArgs(_width, _height));
    }
}