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

        /// Genera una clave AES de 256 bits (32 bytes) desde una contraseña
        /// </summary>
        public static byte[] GenerateAESKey(string password, byte[] salt = null)
        {
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));
            
            if (salt == null)
                salt = RandomNumberGenerator.GetBytes(16);
            using var deriveBytes = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            return deriveBytes.GetBytes(32); // 256 bits
        }

        /// <summary>
        /// Cifra texto plano usando AES-256 en modo CBC
        /// </summary>
        public static string EncryptAES(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            try
            {
                using var aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 256;

                // Generar clave y IV
                var salt = RandomNumberGenerator.GetBytes(16);
                aes.Key = GenerateAESKey(password, salt);
                aes.GenerateIV();

                using var encryptor = aes.CreateEncryptor();
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

                // Formato: salt$iv$encrypted_data (todos en Base64)
                var saltB64 = Convert.ToBase64String(salt);
                var ivB64 = Convert.ToBase64String(aes.IV);
                var encryptedB64 = Convert.ToBase64String(encryptedBytes);

                return $"{saltB64}${ivB64}${encryptedB64}";
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"Error al cifrar texto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Descifra texto cifrado con AES-256
        /// </summary>
        public static string DecryptAES(string encryptedText, string password)
        {
            if (string.IsNullOrEmpty(encryptedText)) return encryptedText;
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            try
            {
                // Separar salt, IV y datos cifrados
                var parts = encryptedText.Split('$');
                if (parts.Length != 3) throw new ArgumentException("Formato de texto cifrado inválido");

                var salt = Convert.FromBase64String(parts[0]);
                var iv = Convert.FromBase64String(parts[1]);
                var encryptedBytes = Convert.FromBase64String(parts[2]);

                using var aes = Aes.Create();
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.KeySize = 256;

                // Reconstruir clave y IV
                aes.Key = GenerateAESKey(password, salt);
                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor();
                var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                throw new CryptographicException($"Error al descifrar texto: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica si un texto está cifrado con AES
        /// </summary>
        public static bool IsAESEncrypted(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            var parts = text.Split('$');
            return parts.Length == 3 && 
                   parts[0].Length > 0 && 
                   parts[1].Length > 0 && 
                   parts[2].Length > 0;
        }

        /// <summary>
        /// Cifra datos sensibles de usuario (email, teléfono, etc.)
        /// </summary>
        public static string EncryptSensitiveData(string sensitiveData, string masterKey)
        {
            if (string.IsNullOrEmpty(sensitiveData)) return sensitiveData;
            return EncryptAES(sensitiveData, masterKey);
        }

        /// <summary>
        /// Descifra datos sensibles de usuario
        /// </summary>
        public static string DecryptSensitiveData(string encryptedData, string masterKey)
        {
            if (string.IsNullOrEmpty(encryptedData)) return encryptedData;
            return DecryptAES(encryptedData, masterKey);
        }

        /// <summary>
        /// Genera una clave maestra segura para el sistema
        /// </summary>
        public static string GenerateMasterKey()
        {
            var keyBytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(keyBytes);
        }
    }
}


