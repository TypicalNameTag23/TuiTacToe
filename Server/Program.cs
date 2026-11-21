namespace Server;

public static class Program
{
    private const int Port = 4343;
    
    public static void Main()
    {
        var server = new Server(Port);
        Console.CancelKeyPress += (_, _) => server.StopAsync().Wait();
        
        server.Start();
    }
}