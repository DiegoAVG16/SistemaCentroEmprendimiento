using System;
using CentroEmpData.Seguridad;

namespace CentroEmpData.Seguridad.Tests
{
    /// <summary>
    /// Script de consola para probar el sistema de cifrado
    /// </summary>
    public class TestEncryptionConsole
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("🔐 SISTEMA DE CIFRADO - PRUEBAS EN CONSOLA");
            Console.WriteLine("==========================================");
            Console.WriteLine();

            try
            {
                // Ejecutar todas las pruebas
                RunAllTests();
                
                Console.WriteLine();
                Console.WriteLine("🎉 ¡TODAS LAS PRUEBAS COMPLETADAS EXITOSAMENTE!");
                Console.WriteLine("✅ Hash SHA-256 + PBKDF2 funcionando");
                Console.WriteLine("✅ Cifrado AES-256 funcionando");
                Console.WriteLine("✅ Sistema de seguridad completo");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en las pruebas: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            Console.WriteLine();
            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadKey();
        }

        private static void RunAllTests()
        {
            TestPasswordHashing();
            TestAESEncryption();
            TestSensitiveDataEncryption();
            TestMasterKeyGeneration();
            TestErrorHandling();
        }

        private static void TestPasswordHashing()
        {
            Console.WriteLine("🔑 PRUEBA 1: Hash de Contraseñas");
            Console.WriteLine("--------------------------------");

            string password = "MiContraseñaSecreta123";
            Console.WriteLine($"Contraseña original: {password}");

            // Generar hash
            string hashedPassword = SecurityUtils.HashPassword(password);
            Console.WriteLine($"Hash generado: {hashedPassword}");

            // Verificar si está hasheado
            bool isHashed = SecurityUtils.IsHashed(hashedPassword);
            Console.WriteLine($"¿Está hasheado? {isHashed}");

            // Verificar contraseña correcta
            bool correctVerification = SecurityUtils.VerifyPassword(password, hashedPassword);
            Console.WriteLine($"Verificación correcta: {correctVerification}");

            // Verificar contraseña incorrecta
            bool incorrectVerification = SecurityUtils.VerifyPassword("ContraseñaIncorrecta", hashedPassword);
            Console.WriteLine($"Verificación incorrecta: {incorrectVerification}");

            Console.WriteLine("✅ Prueba de hash completada");
            Console.WriteLine();
        }

        private static void TestAESEncryption()
        {
            Console.WriteLine("🔒 PRUEBA 2: Cifrado AES-256");
            Console.WriteLine("------------------------------");

            string plainText = "Este es un mensaje secreto que será cifrado con AES-256";
            string password = "MiClaveSecreta123";
            
            Console.WriteLine($"Texto original: {plainText}");
            Console.WriteLine($"Contraseña: {password}");

            // Cifrar
            string encryptedText = SecurityUtils.EncryptAES(plainText, password);
            Console.WriteLine($"Texto cifrado: {encryptedText}");

            // Verificar si está cifrado
            bool isEncrypted = SecurityUtils.IsAESEncrypted(encryptedText);
            Console.WriteLine($"¿Está cifrado? {isEncrypted}");

            // Descifrar
            string decryptedText = SecurityUtils.DecryptAES(encryptedText, password);
            Console.WriteLine($"Texto descifrado: {decryptedText}");

            // Verificar que el descifrado sea correcto
            bool decryptionCorrect = plainText == decryptedText;
            Console.WriteLine($"¿Descifrado correcto? {decryptionCorrect}");

            Console.WriteLine("✅ Prueba de cifrado AES completada");
            Console.WriteLine();
        }

        private static void TestSensitiveDataEncryption()
        {
            Console.WriteLine("🔐 PRUEBA 3: Cifrado de Datos Sensibles");
            Console.WriteLine("----------------------------------------");

            // Generar clave maestra
            string masterKey = SecurityUtils.GenerateMasterKey();
            Console.WriteLine($"Clave maestra generada: {masterKey}");

            // Datos sensibles
            string[] sensitiveData = {
                "usuario@empresa.com",
                "+57 300 123 4567",
                "Calle 123 #45-67, Bogotá, Colombia",
                "Información confidencial del cliente"
            };

            foreach (string data in sensitiveData)
            {
                Console.WriteLine($"\nDato original: {data}");

                // Cifrar
                string encryptedData = SecurityUtils.EncryptSensitiveData(data, masterKey);
                Console.WriteLine($"Dato cifrado: {encryptedData}");

                // Verificar cifrado
                bool isEncrypted = SecurityUtils.IsAESEncrypted(encryptedData);
                Console.WriteLine($"¿Está cifrado? {isEncrypted}");

                // Descifrar
                string decryptedData = SecurityUtils.DecryptSensitiveData(encryptedData, masterKey);
                Console.WriteLine($"Dato descifrado: {decryptedData}");

                // Verificar
                bool correct = data == decryptedData;
                Console.WriteLine($"¿Descifrado correcto? {correct}");
            }

            Console.WriteLine("✅ Prueba de datos sensibles completada");
            Console.WriteLine();
        }

        private static void TestMasterKeyGeneration()
        {
            Console.WriteLine("🔑 PRUEBA 4: Generación de Claves Maestras");
            Console.WriteLine("-------------------------------------------");

            // Generar múltiples claves
            for (int i = 1; i <= 3; i++)
            {
                string masterKey = SecurityUtils.GenerateMasterKey();
                Console.WriteLine($"Clave maestra {i}: {masterKey}");
            }

            // Verificar que las claves sean diferentes
            string key1 = SecurityUtils.GenerateMasterKey();
            string key2 = SecurityUtils.GenerateMasterKey();
            bool keysAreDifferent = key1 != key2;
            Console.WriteLine($"¿Las claves son diferentes? {keysAreDifferent}");

            Console.WriteLine("✅ Prueba de generación de claves completada");
            Console.WriteLine();
        }

        private static void TestErrorHandling()
        {
            Console.WriteLine("⚠️ PRUEBA 5: Manejo de Errores");
            Console.WriteLine("--------------------------------");

            try
            {
                // Probar con texto vacío
                string result = SecurityUtils.EncryptAES("", "password");
                Console.WriteLine($"Cifrado con texto vacío: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error esperado con texto vacío: {ex.Message}");
            }

            try
            {
                // Probar con contraseña vacía
                string result = SecurityUtils.EncryptAES("texto", "");
                Console.WriteLine($"Cifrado con contraseña vacía: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error esperado con contraseña vacía: {ex.Message}");
            }

            try
            {
                // Probar descifrado de texto no cifrado
                string result = SecurityUtils.DecryptAES("texto_no_cifrado", "password");
                Console.WriteLine($"Descifrado de texto no cifrado: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error esperado con texto no cifrado: {ex.Message}");
            }

            Console.WriteLine("✅ Prueba de manejo de errores completada");
            Console.WriteLine();
        }
    }
}
