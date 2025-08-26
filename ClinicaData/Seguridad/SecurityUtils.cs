using System;
using System.Security.Cryptography;
using System.Text;

namespace CentroEmpData.Seguridad
{
    public static class SecurityUtils
    {
        // Formato: pbkdf2_sha256$iterations$salt$hash$keylen
        public static string HashPassword(string plainPassword, int iterations = 100_000, int keyLen = 32)
        {
            if (plainPassword == null) throw new ArgumentNullException(nameof(plainPassword));
            var saltBytes = RandomNumberGenerator.GetBytes(16);
            var hashBytes = Rfc2898DeriveBytes.Pbkdf2(plainPassword, saltBytes, iterations, HashAlgorithmName.SHA256, keyLen);
            var saltHex = Convert.ToHexString(saltBytes).ToLowerInvariant();
            var hashHex = Convert.ToHexString(hashBytes).ToLowerInvariant();
            return $"pbkdf2_sha256${iterations}${saltHex}${hashHex}${keyLen}";
        }

        public static bool IsHashed(string stored)
        {
            if (string.IsNullOrEmpty(stored)) return false;
            return stored.StartsWith("pbkdf2_sha256$");
        }

        public static bool VerifyPassword(string plainPassword, string stored)
        {
            if (plainPassword == null || string.IsNullOrEmpty(stored)) return false;
            var parts = stored.Split('$');
            if (parts.Length < 5 || parts[0] != "pbkdf2_sha256") return false;
            if (!int.TryParse(parts[1], out var iterations)) return false;
            var saltHex = parts[2];
            var storedHashHex = parts[3];
            var keyLen = parts.Length >= 5 && int.TryParse(parts[4], out var kl) ? kl : 32;

            var saltBytes = Convert.FromHexString(saltHex);
            var calcHash = Rfc2898DeriveBytes.Pbkdf2(plainPassword, saltBytes, iterations, HashAlgorithmName.SHA256, keyLen);
            var calcHashHex = Convert.ToHexString(calcHash).ToLowerInvariant();
            return FixedTimeEquals(storedHashHex, calcHashHex);
        }

        private static bool FixedTimeEquals(string aHex, string bHex)
        {
            if (aHex.Length != bHex.Length) return false;
            int diff = 0;
            for (int i = 0; i < aHex.Length; i++) diff |= aHex[i] ^ bHex[i];
            return diff == 0;
        }
    }
}


