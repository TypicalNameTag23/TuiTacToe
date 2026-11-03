using Client.Ssh;

namespace Client;

public static class Program
{
    public static async Task Main()
    {
        var server = new SshServer();
        server.Start();

        await Task.Delay(Timeout.Infinite);
    }
}