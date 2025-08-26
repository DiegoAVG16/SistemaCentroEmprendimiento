## Esquema de seguridad (disponibilidad y confidencialidad)

### Contraseñas (confidencialidad)
- Hash PBKDF2-SHA-256 con sal y 100k iteraciones.
- Implementado en `ClinicaData/Seguridad/SecurityUtils.cs` y aplicado en `UsuarioRepositorio` para `Guardar`, `Editar` y validación en `Login`.

### Copias de seguridad (disponibilidad)
- Programar tarea diaria (Windows Task Scheduler) para exportar la base de datos SQL Server.

Ejemplo PowerShell (ajusta nombre BD/servidor):
```powershell
$date = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$dest = "$PSScriptRoot\\backups"
New-Item -ItemType Directory -Force -Path $dest | Out-Null
sqlcmd -S .\\SQLEXPRESS -Q "BACKUP DATABASE [CentroEmp] TO DISK = '$dest\\CentroEmp_$date.bak' WITH INIT, STATS = 5"
```

Retención sugerida: 7 diarios, 4 semanales, 3 mensuales. Verificar restauración semanal en ambiente de pruebas.

### Cifrado simétrico opcional (AES-256-GCM)
- Para campos sensibles, se recomienda AES-256-GCM con claves en variables de entorno.
- No aplicado aún en la solución; se puede agregar servicio de cifrado en capa de datos si lo requieres.


