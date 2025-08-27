using Microsoft.AspNetCore.Mvc;
using CentroEmpData.Seguridad;
using CentroEmpWeb.Models.DTOs;

namespace CentroEmpWeb.Controllers
{
    public class EncryptionTestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult TestEncryption(string plainText, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(plainText) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Por favor ingresa texto y contraseña";
                    return View("Index");
                }

                // Generar clave maestra
                var masterKey = SecurityUtils.GenerateMasterKey();

                // Cifrar con contraseña personalizada
                var encryptedWithPassword = SecurityUtils.EncryptAES(plainText, password);

                // Cifrar con clave maestra del sistema
                var encryptedWithMasterKey = SecurityUtils.EncryptSensitiveData(plainText, masterKey);

                // Verificar si están cifrados
                var isPasswordEncrypted = SecurityUtils.IsAESEncrypted(encryptedWithPassword);
                var isMasterKeyEncrypted = SecurityUtils.IsAESEncrypted(encryptedWithMasterKey);

                // Descifrar para verificar
                var decryptedWithPassword = SecurityUtils.DecryptAES(encryptedWithPassword, password);
                var decryptedWithMasterKey = SecurityUtils.DecryptSensitiveData(encryptedWithMasterKey, masterKey);

                // Verificar que el descifrado sea correcto
                var passwordDecryptionCorrect = plainText == decryptedWithPassword;
                var masterKeyDecryptionCorrect = plainText == decryptedWithMasterKey;

                ViewBag.Results = new
                {
                    PlainText = plainText,
                    Password = password,
                    MasterKey = masterKey,
                    EncryptedWithPassword = encryptedWithPassword,
                    EncryptedWithMasterKey = encryptedWithMasterKey,
                    IsPasswordEncrypted = isPasswordEncrypted,
                    IsMasterKeyEncrypted = isMasterKeyEncrypted,
                    DecryptedWithPassword = decryptedWithPassword,
                    DecryptedWithMasterKey = decryptedWithMasterKey,
                    PasswordDecryptionCorrect = passwordDecryptionCorrect,
                    MasterKeyDecryptionCorrect = masterKeyDecryptionCorrect
                };

                ViewBag.Success = "Prueba de cifrado completada exitosamente";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error en la prueba: {ex.Message}";
            }

            return View("Index");
        }

        [HttpPost]
        public IActionResult TestHash(string password)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    ViewBag.HashError = "Por favor ingresa una contraseña";
                    return View("Index");
                }

                // Generar hash
                var hashedPassword = SecurityUtils.HashPassword(password);

                // Verificar si está hasheado
                var isHashed = SecurityUtils.IsHashed(hashedPassword);

                // Verificar la contraseña
                var verificationResult = SecurityUtils.VerifyPassword(password, hashedPassword);

                // Verificar con contraseña incorrecta
                var wrongPasswordVerification = SecurityUtils.VerifyPassword("ContraseñaIncorrecta", hashedPassword);

                ViewBag.HashResults = new
                {
                    OriginalPassword = password,
                    HashedPassword = hashedPassword,
                    IsHashed = isHashed,
                    VerificationResult = verificationResult,
                    WrongPasswordVerification = wrongPasswordVerification
                };

                ViewBag.HashSuccess = "Prueba de hash completada exitosamente";
            }
            catch (Exception ex)
            {
                ViewBag.HashError = $"Error en la prueba de hash: {ex.Message}";
            }

            return View("Index");
        }

        [HttpPost]
        public IActionResult GenerateMasterKey()
        {
            try
            {
                var masterKey = SecurityUtils.GenerateMasterKey();
                ViewBag.GeneratedMasterKey = masterKey;
                ViewBag.MasterKeySuccess = "Clave maestra generada exitosamente";
            }
            catch (Exception ex)
            {
                ViewBag.MasterKeyError = $"Error generando clave maestra: {ex.Message}";
            }

            return View("Index");
        }
    }
}
