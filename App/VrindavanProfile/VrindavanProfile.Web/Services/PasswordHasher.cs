using System;
using System.Security.Cryptography;
namespace VrindavanProfile.Web.Services;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 10000;
    private static readonly HashAlgorithmName Algorithm =
        HashAlgorithmName.SHA256;
    
    public static string Hash(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations,
            Algorithm, KeySize);
        return $"{Convert.ToHexString(salt)}:{Convert.ToHexString(hash)}";
    }
    
    public static bool Verify(string password, string hashedPassword)
    {
        string[] parts = hashedPassword.Split(':');
        if (parts.Length != 2) return false;
        byte[] salt = Convert.FromHexString(parts[0]);
        byte[] hash = Convert.FromHexString(parts[1]);
        byte[] testHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations,
            Algorithm, KeySize);
        return CryptographicOperations.FixedTimeEquals(hash, testHash);
    }
}