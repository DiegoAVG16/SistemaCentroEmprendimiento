using Microsoft.Extensions.Options;

namespace CentroEmpData.Seguridad
{
    /// <summary>
    /// Configuración para el sistema de cifrado
    /// </summary>
    public class EncryptionConfig
    {
        public string MasterKey { get; set; } = string.Empty;
        public bool EnableDataEncryption { get; set; } = true;
        public string[] SensitiveFields { get; set; } = new[] { "Correo", "Telefono", "Direccion" };
    }

    /// <summary>
    /// Servicio para manejar la configuración de cifrado
    /// </summary>
    public class EncryptionService
    {
        private readonly EncryptionConfig _config;

        public EncryptionService(IOptions<EncryptionConfig> config)
        {
            _config = config.Value;
        }

        /// <summary>
        /// Cifra un campo si está marcado como sensible
        /// </summary>
        public string EncryptField(string fieldName, string value)
        {
            if (!_config.EnableDataEncryption) return value;
            if (string.IsNullOrEmpty(value)) return value;
            if (!_config.SensitiveFields.Contains(fieldName)) return value;

            return SecurityUtils.EncryptSensitiveData(value, _config.MasterKey);
        }

        /// <summary>
        /// Descifra un campo si está cifrado
        /// </summary>
        public string DecryptField(string fieldName, string value)
        {
            if (!_config.EnableDataEncryption) return value;
            if (string.IsNullOrEmpty(value)) return value;
            if (!_config.SensitiveFields.Contains(fieldName)) return value;

            return SecurityUtils.IsAESEncrypted(value) 
                ? SecurityUtils.DecryptSensitiveData(value, _config.MasterKey) 
                : value;
        }

        /// <summary>
        /// Genera una nueva clave maestra
        /// </summary>
        public string GenerateNewMasterKey()
        {
            var newKey = SecurityUtils.GenerateMasterKey();
            _config.MasterKey = newKey;
            return newKey;
        }
    }
}
