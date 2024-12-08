using BCrypt.Net;

namespace Chess_Final.PasswordManager;

internal static class PasswordManager
{
    internal static string HashPassword(string password)
    {
        string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password, 16);
        return hashedPassword;
    }

    internal static bool VerifyPassword(string password, string hash)
    {
        bool valid = BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
        return valid;
    }
}