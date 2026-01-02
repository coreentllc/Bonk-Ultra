$ErrorActionPreference = "Stop"

param(
  [string]$LogsDir = "Z:\SteamLibrary\steamapps\common\Megabonk\MelonLoader\Logs",
  [string]$OutputPath = "$(Split-Path -Parent $PSScriptRoot)\latest-bonk-ultra.log",
  [string]$Filter = "[BonkUltra]",
  [int]$PollSeconds = 2,
  [switch]$ToClipboard
)

function Copy-LatestLog {
  if (-not (Test-Path $LogsDir)) {
    Write-Host "Log directory not found: $LogsDir"
    return
  }

  $latest = Get-ChildItem -Path $LogsDir -Filter *.log | Sort-Object LastWriteTime -Descending | Select-Object -First 1
  if (-not $latest) {
    return
  }

  $content = Get-Content -Path $latest.FullName -Raw
  if (-not [string]::IsNullOrWhiteSpace($Filter)) {
    $lines = $content -split "(`r`n|`n)"
    $content = ($lines | Where-Object { $_ -like "*$Filter*" }) -join [Environment]::NewLine
  }

  Set-Content -Path $OutputPath -Value $content -Encoding UTF8

  if ($ToClipboard) {
    Set-Clipboard -Value $content
  }

  Write-Host ("Copied {0} -> {1}" -f $latest.Name, $OutputPath)
}

Write-Host "Watching MelonLoader logs in: $LogsDir"
Write-Host "Filtering by: $Filter"
Write-Host "Mirroring newest log to: $OutputPath"
Write-Host "Press Ctrl+C to stop."

$lastPath = ""
$lastWrite = [DateTime]::MinValue

while ($true) {
  if (Test-Path $LogsDir) {
    $latest = Get-ChildItem -Path $LogsDir -Filter *.log | Sort-Object LastWriteTime -Descending | Select-Object -First 1
    if ($latest) {
      if ($latest.FullName -ne $lastPath -or $latest.LastWriteTime -ne $lastWrite) {
        Copy-LatestLog
        $lastPath = $latest.FullName
        $lastWrite = $latest.LastWriteTime
      }
    }
  }

  Start-Sleep -Seconds $PollSeconds
}
