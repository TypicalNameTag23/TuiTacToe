using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Select script to run:");
Console.WriteLine("1: Generate config file for Client SSH Server keys");
var input = Console.ReadLine();

switch (input)
{
    case "1":
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var baseDirectory = currentDirectory[..(currentDirectory.IndexOf("TuiTacToe", StringComparison.InvariantCultureIgnoreCase) + 9)];
        Console.WriteLine(baseDirectory);
        var clientDirectory = Path.Join(baseDirectory, "Client");
        if (!Directory.Exists(clientDirectory))
        {
            Console.WriteLine("Unable to find Client project directory, does it exist?");
            return;
        }

        FileStream fileStream;
        var configFilePath = Path.Combine(clientDirectory, "config.txt");
        if (!File.Exists(configFilePath))
        {
            fileStream = File.Create(configFilePath);
        }
        else
        {
            fileStream = File.OpenWrite(configFilePath);
        }
        
        var rsa = RSA.Create(2048);
        var ecdsa256 = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var ecdsa384 = ECDsa.Create(ECCurve.NamedCurves.nistP384);
        var ecdsa521 = ECDsa.Create(ECCurve.NamedCurves.nistP521);
        
        fileStream.Write(Encoding.ASCII.GetBytes($"RSA_KEY{Environment.NewLine}{rsa.ExportPkcs8PrivateKeyPem()}{Environment.NewLine}"));
        fileStream.Write(Encoding.ASCII.GetBytes($"EDCSA_NISTP256_KEY{Environment.NewLine}{ecdsa256.ExportPkcs8PrivateKeyPem()}{Environment.NewLine}"));
        fileStream.Write(Encoding.ASCII.GetBytes($"EDCSA_NISTP384_KEY{Environment.NewLine}{ecdsa384.ExportPkcs8PrivateKeyPem()}{Environment.NewLine}"));
        fileStream.Write(Encoding.ASCII.GetBytes($"EDCSA_NISTP521_KEY{Environment.NewLine}{ecdsa521.ExportPkcs8PrivateKeyPem()}{Environment.NewLine}"));
        fileStream.Close();
        fileStream.Dispose();
        rsa.Dispose();
        ecdsa256.Dispose();
        ecdsa384.Dispose();
        ecdsa521.Dispose();
        
        Console.WriteLine("Created config file.");
        
        break;
    }
    default:
    {
        Console.WriteLine("Unknown input.");
        break;
    }
}