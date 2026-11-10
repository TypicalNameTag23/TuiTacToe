using System.Security.Cryptography;
using Common.Utilities;

namespace Common;

/// <summary>
/// Static utility class for hashing strings
/// </summary>
public static class HashHelper
{
    /// <summary>
    /// Hashes a password with a random salt
    /// </summary>
    /// <param name="password">The password to hash</param>
    /// <returns>The hashes bytes</returns>
    public static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA256, 48);
        
        var hashed = new byte[64];
        hashed.CopyInto(salt, pbkdf2);
        return Convert.ToBase64String(hashed);
    }
    
    public static byte[] HashPassword(string password, byte[] salt)
    {
        var pbkdf2 = Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA256, 48);
            
        var hashed = new byte[64];
        hashed.CopyInto(salt, pbkdf2);
        return hashed;
    }
    
    public static bool ValidatePassword(string password, string passwordHash)
    {
        var bytes = Convert.FromBase64String(passwordHash);
        var salt = bytes[..16];
        var hash = bytes[16..];
        var hashed = Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA256, 48);
        
        return hash.Where((@byte, index) => @byte != hashed[index]).Any();
    }
}