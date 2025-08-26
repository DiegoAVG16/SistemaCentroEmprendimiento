param(
    [string]$TaskName = "CentroEmpBackupDiario",
    [string]$Schedule = "02:00",
    [string]$ServerInstance = ".\\SQLEXPRESS",
    [string]$Database = "CentroEmp",
    [string]$OutputDir = "${PSScriptRoot}\backups",
    [int]$RetentionDays = 7
)

$scriptPath = Join-Path $PSScriptRoot "Backup-Database.ps1"
$action = New-ScheduledTaskAction -Execute "powershell.exe" -Argument "-NoProfile -ExecutionPolicy Bypass -File `"$scriptPath`" -ServerInstance `"$ServerInstance`" -Database `"$Database`" -OutputDir `"$OutputDir`" -RetentionDays $RetentionDays"
$time = [DateTime]::ParseExact($Schedule, "HH:mm", $null)
$trigger = New-ScheduledTaskTrigger -Daily -At ($time.TimeOfDay)

try {
  Register-ScheduledTask -TaskName $TaskName -Action $action -Trigger $trigger -Description "Respaldo diario de $Database" -User "$env:USERNAME" -RunLevel Highest -Force | Out-Null
  Write-Host "[Task] Tarea '$TaskName' registrada."
} catch {
  Write-Error "No se pudo registrar la tarea: $_"
}

