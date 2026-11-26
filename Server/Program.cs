using System.Net;
using System.Security.Cryptography.X509Certificates;
using Common.Utilities;
using Npgsql;
using Server.Data;
using Server.Data.Models;
using SshServer;

namespace Server;

public static class Program
{
    private const int Port = 4343;
    
    public static void Main()
    {
        try
        {
            var directory = Directory.GetCurrentDirectory();
            while (!directory.EndsWith("Server") && directory != $"{Path.PathSeparator}")
            {
                directory = Directory.GetParent(directory)!.FullName;
            }
            
            var configReader = new ConfigReader(Path.Join(directory, "config.txt"));
            var cert = X509Certificate2.CreateFromPemFile(Path.Combine(directory, "cert.pem"), Path.Combine(directory, "key.pem"));
            // windows doesnt support certs from pem files
            // as a linux user this doesnt impact me but whatever ig
            cert = X509CertificateLoader.LoadPkcs12(cert.Export(X509ContentType.Pkcs12), null);

            var connString = configReader.ReadValue("CONNECTION_STRING");
            using var database = new NpgsqlDataSourceBuilder(connString).Build();
            var server = new Server(Port, cert, new UserRepository(database), new LeaderboardSource(database));
            
            Console.CancelKeyPress += (_, _) =>
            {
                server.StopAsync().Wait();
                server.Dispose();
            };
        
            server.Start();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Failed to start, did you create the config file using the script?");
        }  
    }
}