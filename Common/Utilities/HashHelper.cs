using System.Security.Cryptography;

namespace Common.Utilities;

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
    public static byte[] HashPassword(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(64);
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA256, 256);
    }
    
    public static byte[] HashPassword(string password, byte[] salt)
    {
        return Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA256, 256);
    }
}