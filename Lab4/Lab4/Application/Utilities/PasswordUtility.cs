using System.Security.Cryptography;

namespace Lab4.Application.Utilities;

public static class PasswordUtility
{
    public static string GetHashedPassword(string password, string salt)
    {
        string saltedPassword = password + salt;
        using SHA256 sha256 = SHA256.Create();
        byte[] hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(saltedPassword));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    public static string CreatePasswordSalt()
    {
        // Generate a random salt for password hashing
        byte[] saltBytes = new byte[16];
        saltBytes = RandomNumberGenerator.GetBytes(16);

        return Convert.ToBase64String(saltBytes);
    }
}