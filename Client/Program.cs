using System.Net;
using Client.Ssh;

namespace Client;

public static class Program
{
    public static readonly IPAddress BackendServerAddress = IPAddress.Loopback;
    public const ushort BackendServerPort = 4343;
    private const ushort Port = 22;
    
    public static async Task Main()
    {
        var server = new SshServer(Port, BackendServerAddress, BackendServerPort);
        server.Start();
        
        Console.CancelKeyPress += (_, _) => server.Stop();
        await Task.Delay(Timeout.Infinite);
    }
}