using System.Net;
using SshClient.Ssh;

namespace SshClient;

public static class Program
{
    private static readonly IPAddress BackendServerAddress = IPAddress.Parse("127.0.0.1");
    private const ushort BackendServerPort = 4343;
    private const ushort Port = 4342;
    
    public static async Task Main()
    {
        var server = new SshServer(Port, BackendServerAddress, BackendServerPort);
        server.Start();
        
        Console.CancelKeyPress += (_, _) => server.Stop();
        await Task.Delay(Timeout.Infinite);
    }
}