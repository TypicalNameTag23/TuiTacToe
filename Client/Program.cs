using Client.Ssh;
using Common.Packets.Handshake;

namespace Client;

public static class Program
{
    public static async Task Main()
    {
        var server = new SshServer();
        server.Start();
        
        Console.CancelKeyPress += (_, _) => server.Stop();
        await Task.Delay(Timeout.Infinite);
    }
}