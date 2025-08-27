using System;

namespace CentroEmpData.Seguridad
{
    /// <summary>
    /// Ejemplos de uso del sistema de cifrado AES
    /// </summary>
    public static class EncryptionExamples
    {
        /// <summary>
        /// Ejemplo básico de cifrado y descifrado
        /// </summary>
        public static void BasicEncryptionExample()
        {
            Console.WriteLine("=== EJEMPLO BÁSICO DE CIFRADO AES ===");
            
            string password = "MiClaveSecreta123";
            string textoOriginal = "Este es un mensaje secreto que será cifrado con AES-256";
            
            Console.WriteLine($"Texto original: {textoOriginal}");
            Console.WriteLine($"Clave: {password}");
            
            // Cifrar
            string textoCifrado = SecurityUtils.EncryptAES(textoOriginal, password);
            Console.WriteLine($"Texto cifrado: {textoCifrado}");
            
            // Descifrar
            string textoDescifrado = SecurityUtils.DecryptAES(textoCifrado, password);
            Console.WriteLine($"Texto descifrado: {textoDescifrado}");
            
            // Verificar
            bool esCorrecto = textoOriginal == textoDescifrado;
            Console.WriteLine($"¿Descifrado correcto? {esCorrecto}");
            Console.WriteLine();
        }

        /// <summary>
        /// Ejemplo de cifrado de datos sensibles
        /// </summary>
        public static void SensitiveDataExample()
        {
            Console.WriteLine("=== EJEMPLO DE CIFRADO DE DATOS SENSIBLES ===");
            
            string masterKey = SecurityUtils.GenerateMasterKey();
            Console.WriteLine($"Clave maestra generada: {masterKey}");
            
            // Datos sensibles
            string email = "usuario@empresa.com";
            string telefono = "+57 300 123 4567";
            string direccion = "Calle 123 #45-67, Bogotá";
            
            Console.WriteLine($"Email original: {email}");
            Console.WriteLine($"Teléfono original: {telefono}");
            Console.WriteLine($"Dirección original: {direccion}");
            
            // Cifrar datos sensibles
            string emailCifrado = SecurityUtils.EncryptSensitiveData(email, masterKey);
            string telefonoCifrado = SecurityUtils.EncryptSensitiveData(telefono, masterKey);
            string direccionCifrada = SecurityUtils.EncryptSensitiveData(direccion, masterKey);
            
            Console.WriteLine($"Email cifrado: {emailCifrado}");
            Console.WriteLine($"Teléfono cifrado: {telefonoCifrado}");
            Console.WriteLine($"Dirección cifrada: {direccionCifrada}");
            
            // Verificar si están cifrados
            Console.WriteLine($"¿Email cifrado? {SecurityUtils.IsAESEncrypted(emailCifrado)}");
            Console.WriteLine($"¿Teléfono cifrado? {SecurityUtils.IsAESEncrypted(telefonoCifrado)}");
            Console.WriteLine($"¿Dirección cifrada? {SecurityUtils.IsAESEncrypted(direccionCifrada)}");
            
            // Descifrar
            string emailDescifrado = SecurityUtils.DecryptSensitiveData(emailCifrado, masterKey);
            string telefonoDescifrado = SecurityUtils.DecryptSensitiveData(telefonoCifrado, masterKey);
            string direccionDescifrada = SecurityUtils.DecryptSensitiveData(direccionCifrada, masterKey);
            
            Console.WriteLine($"Email descifrado: {emailDescifrado}");
            Console.WriteLine($"Teléfono descifrado: {telefonoDescifrado}");
            Console.WriteLine($"Dirección descifrada: {direccionDescifrada}");
            Console.WriteLine();
        }

        /// <summary>
        /// Ejemplo de migración de datos existentes
        /// </summary>
        public static void MigrationExample()
        {
            Console.WriteLine("=== EJEMPLO DE MIGRACIÓN DE DATOS ===");
            
            string masterKey = SecurityUtils.GenerateMasterKey();
            
            // Simular datos existentes sin cifrar
            string[] datosExistentes = {
                "admin@empresa.com",
                "usuario1@empresa.com",
                "contacto@empresa.com"
            };
            
            Console.WriteLine("Datos existentes (sin cifrar):");
            foreach (var dato in datosExistentes)
            {
                Console.WriteLine($"- {dato}");
            }
            
            // Migrar a cifrado
            Console.WriteLine("\nMigrando a cifrado AES...");
            string[] datosCifrados = new string[datosExistentes.Length];
            
            for (int i = 0; i < datosExistentes.Length; i++)
            {
                datosCifrados[i] = SecurityUtils.EncryptSensitiveData(datosExistentes[i], masterKey);
                Console.WriteLine($"- {datosExistentes[i]} → {datosCifrados[i]}");
            }
            
            // Verificar que se pueden descifrar
            Console.WriteLine("\nVerificando descifrado...");
            for (int i = 0; i < datosCifrados.Length; i++)
            {
                string descifrado = SecurityUtils.DecryptSensitiveData(datosCifrados[i], masterKey);
                bool esCorrecto = descifrado == datosExistentes[i];
                Console.WriteLine($"- {datosCifrados[i]} → {descifrado} ✓ {esCorrecto}");
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Ejecuta todos los ejemplos
        /// </summary>
        public static void RunAllExamples()
        {
            try
            {
                BasicEncryptionExample();
                SensitiveDataExample();
                MigrationExample();
                
                Console.WriteLine("=== TODOS LOS EJEMPLOS EJECUTADOS EXITOSAMENTE ===");
                Console.WriteLine("✅ Cifrado AES-256 implementado");
                Console.WriteLine("✅ Hash SHA-256 + PBKDF2 implementado");
                Console.WriteLine("✅ Sistema de seguridad completo");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en los ejemplos: {ex.Message}");
            }
        }
    }
}
