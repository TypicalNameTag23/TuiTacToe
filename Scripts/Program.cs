using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

Console.WriteLine("Select script to run:");
Console.WriteLine("1: Generate config file for Client SSH Server keys");
var input = Console.ReadLine();

switch (input)
{
    case "1":
    {
        var baseDirectory = Directory.GetCurrentDirectory();
        while (!baseDirectory.EndsWith("TuiTacToe") && baseDirectory != $"{Path.PathSeparator}")
        {
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;
        }
        
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
    case "2":
    {
        var baseDirectory = Directory.GetCurrentDirectory();
        while (!baseDirectory.EndsWith("TuiTacToe") && baseDirectory != $"{Path.PathSeparator}")
        {
            baseDirectory = Directory.GetParent(baseDirectory)!.FullName;
        }
        
        Console.WriteLine(baseDirectory);
        var clientDirectory = Path.Join(baseDirectory, "Client");
        if (!Directory.Exists(clientDirectory))
        {
            Console.WriteLine("Unable to find Client project directory, does it exist?");
            return;
        }

        FileStream fileStream;
        var configFilePath = Path.Combine(clientDirectory, "cert.pem");
        if (!File.Exists(configFilePath))
        {
            fileStream = File.Create(configFilePath);
        }
        else
        {
            File.WriteAllText(configFilePath, string.Empty);
            fileStream = File.OpenWrite(configFilePath);
        }

        using var key = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        var req = new CertificateRequest($"CN=localhost", key, HashAlgorithmName.SHA256);
        using var cert = req.CreateSelfSigned(DateTimeOffset.UtcNow.AddMinutes(-5),
            DateTimeOffset.UtcNow.AddYears(5));
        
        using var cert2 = X509CertificateLoader.LoadPkcs12(cert.Export(X509ContentType.Pkcs12), null);
        
        fileStream.Write(Encoding.ASCII.GetBytes(cert2.ExportCertificatePem()));
        fileStream.Close();

        Console.WriteLine(cert2.GetRSAPrivateKey()!.ExportRSAPrivateKeyPem());
        var pem2 = X509Certificate2.CreateFromPem(File.ReadAllText(Path.Combine(clientDirectory, "cert.pem")));
        
        break;
    }
    default:
    {
        Console.WriteLine("Unknown input.");
        break;
    }
}