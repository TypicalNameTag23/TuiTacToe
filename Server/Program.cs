namespace Server;

public static class Program
{
    public static void Main()
    {
        var server = new Server();
        Console.CancelKeyPress += (_, _) => server.Stop();
        
        server.Start();
    }
}