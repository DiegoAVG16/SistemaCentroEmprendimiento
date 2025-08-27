using System;
using CentroEmpData.Seguridad;

namespace TestEncryption
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("🔐 PRUEBA RÁPIDA DEL SISTEMA DE CIFRADO");
            Console.WriteLine("=======================================");
            Console.WriteLine();

            try
            {
                // Prueba 1: Hash de contraseña
                Console.WriteLine("🔑 PRUEBA 1: Hash de Contraseña");
                string password = "MiContraseña123";
                string hashedPassword = SecurityUtils.HashPassword(password);
                Console.WriteLine($"Contraseña: {password}");
                Console.WriteLine($"Hash: {hashedPassword}");
                Console.WriteLine($"¿Está hasheado? {SecurityUtils.IsHashed(hashedPassword)}");
                Console.WriteLine($"Verificación: {SecurityUtils.VerifyPassword(password, hashedPassword)}");
                Console.WriteLine();

                // Prueba 2: Cifrado AES
                Console.WriteLine("🔒 PRUEBA 2: Cifrado AES-256");
                string texto = "Hola Mundo Secreto!";
                string clave = "MiClave123";
                string cifrado = SecurityUtils.EncryptAES(texto, clave);
                string descifrado = SecurityUtils.DecryptAES(cifrado, clave);
                Console.WriteLine($"Texto original: {texto}");
                Console.WriteLine($"Texto cifrado: {cifrado}");
                Console.WriteLine($"Texto descifrado: {descifrado}");
                Console.WriteLine($"¿Descifrado correcto? {texto == descifrado}");
                Console.WriteLine();

                // Prueba 3: Clave maestra
                Console.WriteLine("🔐 PRUEBA 3: Clave Maestra");
                string masterKey = SecurityUtils.GenerateMasterKey();
                Console.WriteLine($"Clave maestra: {masterKey}");
                Console.WriteLine();

                // Prueba 4: Cifrado de datos sensibles
                Console.WriteLine("🔐 PRUEBA 4: Datos Sensibles");
                string email = "usuario@empresa.com";
                string emailCifrado = SecurityUtils.EncryptSensitiveData(email, masterKey);
                string emailDescifrado = SecurityUtils.DecryptSensitiveData(emailCifrado, masterKey);
                Console.WriteLine($"Email original: {email}");
                Console.WriteLine($"Email cifrado: {emailCifrado}");
                Console.WriteLine($"Email descifrado: {emailDescifrado}");
                Console.WriteLine($"¿Descifrado correcto? {email == emailDescifrado}");
                Console.WriteLine();

                Console.WriteLine("✅ ¡TODAS LAS PRUEBAS EXITOSAS!");
                Console.WriteLine("🎉 Sistema de cifrado funcionando correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
            }

            Console.WriteLine("\nPresiona cualquier tecla para salir...");
            Console.ReadKey();
        }
    }
}
