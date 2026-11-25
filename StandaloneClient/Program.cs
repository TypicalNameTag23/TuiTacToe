using System.Net;
using Client;
using Painter.Platform;

public class Program
{
    public static async Task Main()
    {
        var client = new Application(Terminal.GetPlatformTerminal(), IPAddress.Loopback, 4343);
        await client.StartAsync();
    }
}