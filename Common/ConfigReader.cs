using System.Text;

namespace SshServer;

/// <summary>
/// Basic config reader
/// </summary>
/// <param name="filePath">The file path of the config</param>
public class ConfigReader
{
    private Dictionary<string, string> configValues = [];
    
    public ConfigReader(string filePath)
    {
        using var reader = File.OpenText(filePath);
        while (!reader.EndOfStream)
        {
            var configEntry = reader.ReadLine()!;
            
            var line = reader.ReadLine();
            if (line == null)
            {
                throw new Exception($"Unable to find value associated with config entry {configEntry}");
            }
            
            var sb = new StringBuilder(line);
            while (!reader.EndOfStream && !string.IsNullOrEmpty(line))
            {
                line = reader.ReadLine();
                sb.AppendLine(line);
            }
            
            configValues[configEntry] = sb.ToString();
        }
    }

    /// <summary>
    /// Name of the key to be read
    /// </summary>
    /// <param name="value">Name of the key</param>
    /// <returns>The key, or null if not found</returns>
    public string? ReadValue(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }
        
        return configValues.GetValueOrDefault(value);
    }
}