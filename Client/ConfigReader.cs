namespace Client;

/// <summary>
/// Basic config reader made to read keys from a file
/// </summary>
/// <param name="filePath">The file path of the config</param>
public class ConfigReader(string filePath)
{
    private const string BeginPrivateKeyText = "-----BEGIN PRIVATE KEY-----"; 
    private const string EndPrivateKeyText = "-----END PRIVATE KEY-----"; 
    // private const string EdcsaNistp256KeyText = "EDCSA_NISTP256_KEY"; 
    // private const string EdcsaNistp384KeyText = "EDCSA_NISTP384_KEY"; 
    // private const string EdcsaNistp521KeyText = "EDCSA_NISTP521_KEY"; 
    
    private readonly string _fileContents = File.ReadAllText(filePath);

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
        
        var index = _fileContents.IndexOf(value, StringComparison.InvariantCultureIgnoreCase);
        if (index == -1)
        {
            return null;
        }

        var keyStartIndex = index + value.Length + 1;
        var keyEndIndex = _fileContents.IndexOf(
            EndPrivateKeyText, 
            keyStartIndex, 
            StringComparison.InvariantCultureIgnoreCase);

        return _fileContents.Substring(keyStartIndex, keyEndIndex + EndPrivateKeyText.Length - keyStartIndex);
    }
}