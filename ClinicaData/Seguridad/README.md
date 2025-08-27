# 🔐 Sistema de Seguridad - Centro de Emprendimiento UNIANDES

## 📋 Resumen de Implementaciones

Este sistema implementa **dos capas de seguridad** para proteger la información de los usuarios:

### ✅ 1. Hash de Contraseñas con SHA-256 + PBKDF2
- **Algoritmo:** PBKDF2 con SHA-256
- **Salt:** 16 bytes aleatorios únicos por usuario
- **Iteraciones:** 100,000 (configurable)
- **Longitud de clave:** 32 bytes (256 bits)
- **Formato almacenado:** `pbkdf2_sha256$100000$salt$hash$32`

### ✅ 2. Cifrado Simétrico con AES-256
- **Algoritmo:** AES-256 en modo CBC
- **Padding:** PKCS7
- **Salt:** 16 bytes aleatorios por operación
- **IV:** Vector de inicialización único por operación
- **Formato almacenado:** `salt$iv$encrypted_data` (Base64)

## 🏗️ Arquitectura del Sistema

```
SecurityUtils.cs          ← Clase principal con métodos de seguridad
├── HashPassword()        ← Genera hash de contraseñas
├── VerifyPassword()      ← Verifica contraseñas
├── EncryptAES()         ← Cifra texto con AES-256
├── DecryptAES()         ← Descifra texto con AES-256
└── GenerateMasterKey()  ← Genera claves maestras

EncryptionConfig.cs       ← Configuración del sistema de cifrado
├── MasterKey            ← Clave maestra del sistema
├── EnableDataEncryption ← Habilita/deshabilita cifrado
└── SensitiveFields      ← Campos marcados como sensibles

EncryptionService.cs      ← Servicio para manejar cifrado automático
├── EncryptField()       ← Cifra campos sensibles automáticamente
├── DecryptField()       ← Descifra campos automáticamente
└── GenerateNewMasterKey() ← Genera nueva clave maestra

EncryptionExamples.cs     ← Ejemplos de uso del sistema
```

## 🚀 Cómo Usar el Sistema

### **1. Hash de Contraseñas (Automático)**

```csharp
// Al crear/editar usuario - se hace automáticamente
var usuario = new Usuario 
{
    NumeroDocumentoIdentidad = "123456789",
    Clave = "MiContraseña123" // Se hashea automáticamente
};

// Al hacer login - se verifica automáticamente
var loginExitoso = await _repositorio.Login("123456789", "MiContraseña123");
```

### **2. Cifrado AES Manual**

```csharp
// Cifrar texto
string textoOriginal = "Información confidencial";
string clave = "MiClaveSecreta123";
string textoCifrado = SecurityUtils.EncryptAES(textoOriginal, clave);

// Descifrar texto
string textoDescifrado = SecurityUtils.DecryptAES(textoCifrado, clave);
```

### **3. Cifrado Automático de Datos Sensibles**

```csharp
// Configurar servicio de cifrado
var encryptionService = new EncryptionService(config);

// Cifrar campo sensible
string emailCifrado = encryptionService.EncryptField("Correo", "usuario@empresa.com");

// Descifrar campo sensible
string emailDescifrado = encryptionService.DecryptField("Correo", emailCifrado);
```

## 🔧 Configuración

### **appsettings.json**
```json
{
  "EncryptionConfig": {
    "MasterKey": "TuClaveMaestraAqui",
    "EnableDataEncryption": true,
    "SensitiveFields": ["Correo", "Telefono", "Direccion"]
  }
}
```

### **Program.cs**
```csharp
// Registrar servicios de cifrado
builder.Services.Configure<EncryptionConfig>(
    builder.Configuration.GetSection("EncryptionConfig"));
builder.Services.AddScoped<EncryptionService>();
```

## 📊 Características de Seguridad

### **Hash de Contraseñas:**
- ✅ **Salt único** por usuario
- ✅ **100,000 iteraciones** para resistencia a ataques
- ✅ **SHA-256** como función hash base
- ✅ **256 bits** de salida
- ✅ **Comparación segura** en tiempo constante

### **Cifrado AES:**
- ✅ **AES-256** (estándar militar)
- ✅ **Salt único** por operación
- ✅ **IV único** por operación
- ✅ **Modo CBC** para mayor seguridad
- ✅ **Padding PKCS7** estándar

### **Gestión de Claves:**
- ✅ **Clave maestra** del sistema
- ✅ **Generación automática** de claves
- ✅ **Rotación de claves** soportada
- ✅ **Configuración flexible** de campos sensibles

## 🧪 Pruebas y Ejemplos

### **Ejecutar Ejemplos:**
```csharp
// Ejecutar todos los ejemplos
EncryptionExamples.RunAllExamples();

// Ejemplo específico
EncryptionExamples.BasicEncryptionExample();
EncryptionExamples.SensitiveDataExample();
EncryptionExamples.MigrationExample();
```

### **Verificar Funcionamiento:**
```csharp
// Verificar hash
bool esHash = SecurityUtils.IsHashed(contraseñaAlmacenada);

// Verificar cifrado AES
bool esCifrado = SecurityUtils.IsAESEncrypted(textoAlmacenado);
```

## 🔒 Mejores Prácticas

### **1. Contraseñas:**
- ✅ **Nunca almacenar** contraseñas en texto plano
- ✅ **Usar salt único** por usuario
- ✅ **Múltiples iteraciones** (100,000+)
- ✅ **Verificar en tiempo constante**

### **2. Datos Sensibles:**
- ✅ **Cifrar emails** y información personal
- ✅ **Usar claves maestras** del sistema
- ✅ **Rotar claves** periódicamente
- ✅ **Auditar acceso** a datos cifrados

### **3. Seguridad General:**
- ✅ **Mantener actualizado** .NET Framework
- ✅ **Usar HTTPS** en producción
- ✅ **Validar entradas** del usuario
- ✅ **Logging de seguridad** para auditoría

## 📈 Migración de Datos Existentes

### **1. Contraseñas:**
```csharp
// Migración automática al hacer login
if (!SecurityUtils.IsHashed(usuario.Clave))
{
    // La contraseña está en texto plano
    usuario.Clave = SecurityUtils.HashPassword(contraseñaIngresada);
    await _repositorio.Editar(usuario);
}
```

### **2. Datos Sensibles:**
```csharp
// Migrar emails existentes a cifrado
var usuarios = await _repositorio.Lista();
foreach (var usuario in usuarios)
{
    if (!SecurityUtils.IsAESEncrypted(usuario.Correo))
    {
        usuario.Correo = SecurityUtils.EncryptSensitiveData(usuario.Correo, masterKey);
        await _repositorio.Editar(usuario);
    }
}
```

## 🎯 Casos de Uso

### **1. Registro de Usuario:**
- Contraseña se hashea automáticamente
- Email se cifra si está configurado como sensible

### **2. Login de Usuario:**
- Verificación de hash de contraseña
- Migración automática de contraseñas en texto plano

### **3. Edición de Perfil:**
- Contraseñas nuevas se hashean
- Datos sensibles se cifran automáticamente

### **4. Recuperación de Datos:**
- Datos sensibles se descifran automáticamente
- Contraseñas nunca se descifran (solo se verifican)

## 🚨 Consideraciones de Seguridad

### **1. Clave Maestra:**
- ⚠️ **Nunca hardcodear** en el código
- ⚠️ **Almacenar de forma segura** (Azure Key Vault, AWS KMS)
- ⚠️ **Rotar periódicamente** (cada 90 días)
- ⚠️ **Backup seguro** de claves

### **2. Logs y Auditoría:**
- ⚠️ **No loggear** contraseñas o datos cifrados
- ⚠️ **Registrar intentos** de login fallidos
- ⚠️ **Auditar acceso** a datos sensibles
- ⚠️ **Monitorear** patrones sospechosos

### **3. Rendimiento:**
- ⚠️ **Hash de contraseñas** es lento por diseño
- ⚠️ **Cifrado AES** es rápido pero consume recursos
- ⚠️ **Considerar caché** para datos frecuentemente accedidos
- ⚠️ **Monitorear** tiempos de respuesta

## 📚 Recursos Adicionales

- [Microsoft Security Best Practices](https://docs.microsoft.com/en-us/azure/security/)
- [OWASP Password Storage Cheat Sheet](https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html)
- [NIST Digital Identity Guidelines](https://pages.nist.gov/800-63-3/)
- [AES Encryption Standard](https://nvlpubs.nist.gov/nistpubs/FIPS/NIST.FIPS.197.pdf)

---

## 🎉 ¡Sistema de Seguridad Completado!

Tu aplicación ahora tiene:
- ✅ **Hash seguro** de contraseñas con SHA-256 + PBKDF2
- ✅ **Cifrado AES-256** para datos sensibles
- ✅ **Gestión automática** de claves y salt
- ✅ **Migración automática** de datos existentes
- ✅ **Configuración flexible** y segura
- ✅ **Ejemplos completos** de uso
- ✅ **Documentación detallada** del sistema

**¡Tu sistema está listo para manejar datos sensibles de forma segura!** 🔐✨
