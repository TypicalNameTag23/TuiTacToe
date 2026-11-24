using System.Security.Cryptography.X509Certificates;

namespace Server;

public static class Program
{
    private const int Port = 4343;
    
    public static void Main()
    {
        var directory = Directory.GetCurrentDirectory();
        while (!directory.EndsWith("Server") && directory != $"{Path.PathSeparator}")
        {
            directory = Directory.GetParent(directory)!.FullName;
        }
        
        
        var cert = X509Certificate2.CreateFromEncryptedPemFile(Path.Combine(directory, "cert.pem"), "password", Path.Combine(directory, "key.pem"));
        var server = new Server(Port, cert);
        Console.CancelKeyPress += (_, _) => server.StopAsync().Wait();
        
        server.Start();
    }
}