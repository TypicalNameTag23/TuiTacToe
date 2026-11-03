namespace Server;

public static class Program
{
    public static void Main(string[] args)
    {
        var server = new global::Server.Server();
        Console.CancelKeyPress += (_, _) => server.Stop();
        
        server.Start();
    }
}