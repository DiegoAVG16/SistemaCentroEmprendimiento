# Requiere: sqlcmd instalado (SQL Server Command Line Utilities)
param(
    [string]$ServerInstance = ".\\SQLEXPRESS",
    [string]$Database = "CentroEmp",
    [string]$OutputDir = "${PSScriptRoot}\backups",
    [int]$RetentionDays = 7
)

New-Item -ItemType Directory -Force -Path $OutputDir | Out-Null
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$backupFile = Join-Path $OutputDir "$($Database)_$($timestamp).bak"

Write-Host "[Backup] Iniciando respaldo de $Database en $ServerInstance ..."
& sqlcmd -S $ServerInstance -Q "BACKUP DATABASE [$Database] TO DISK = N'$backupFile' WITH INIT, COMPRESSION, STATS = 5" | Out-Null
if ($LASTEXITCODE -ne 0) { Write-Error "Fallo el respaldo"; exit 1 }
Write-Host "[Backup] Respaldo creado: $backupFile"

# Limpieza por retención
Get-ChildItem -Path $OutputDir -Filter "${Database}_*.bak" -File |
  Where-Object { $_.LastWriteTime -lt (Get-Date).AddDays(-$RetentionDays) } |
  Remove-Item -Force -ErrorAction SilentlyContinue

Write-Host "[Backup] Limpieza de archivos antiguos completa (retención: $RetentionDays días)."

