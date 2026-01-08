param(
  [string]$LogsDir = "Z:\SteamLibrary\steamapps\common\Megabonk\MelonLoader\Logs",
  [string]$OutputPath = "$(Split-Path -Parent $PSScriptRoot)\latest-bonk-ultra.log",
  [string]$Filter = "[BonkUltra],[BonkUltraAutoBanish]",
  [string]$BreadcrumbPath = "Z:\SteamLibrary\steamapps\common\Megabonk\UserData\BonkUltraAlpha\last-breadcrumb.txt",
  [string]$BreadcrumbOutputPath = "$(Split-Path -Parent $PSScriptRoot)\latest-bonk-ultra-breadcrumb.log",
  [int]$PollSeconds = 2,
  [switch]$ToClipboard
)

$ErrorActionPreference = "Stop"

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
  $filterList = @()
  if (-not [string]::IsNullOrWhiteSpace($Filter)) {
    $filterList = $Filter -split "," | ForEach-Object { $_.Trim() } | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
  }

  if ($filterList.Count -gt 0) {
    $lines = $content -split "(`r`n|`n)"
    $content = ($lines | Where-Object {
      foreach ($pattern in $filterList) {
        if ($_ -like "*$pattern*") {
          return $true
        }
      }

      return $false
    }) -join [Environment]::NewLine
  }

  Set-Content -Path $OutputPath -Value $content -Encoding UTF8

  if ($ToClipboard) {
    Set-Clipboard -Value $content
  }

  Write-Host ("Copied {0} -> {1}" -f $latest.Name, $OutputPath)
}

function Copy-Breadcrumb {
  if (-not (Test-Path $BreadcrumbPath)) {
    return
  }

  $content = Get-Content -Path $BreadcrumbPath -Raw
  Set-Content -Path $BreadcrumbOutputPath -Value $content -Encoding UTF8
  Write-Host ("Copied breadcrumb -> {0}" -f $BreadcrumbOutputPath)
}

Write-Host "Watching MelonLoader logs in: $LogsDir"
Write-Host "Filtering by: $Filter"
Write-Host "Mirroring newest log to: $OutputPath"
Write-Host "Mirroring breadcrumb to: $BreadcrumbOutputPath"
Write-Host "Press Ctrl+C to stop."

$lastPath = ""
$lastWrite = [DateTime]::MinValue
$breadcrumbWrite = [DateTime]::MinValue

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

  if (Test-Path $BreadcrumbPath) {
    $breadcrumbItem = Get-Item -Path $BreadcrumbPath
    if ($breadcrumbItem.LastWriteTime -ne $breadcrumbWrite) {
      Copy-Breadcrumb
      $breadcrumbWrite = $breadcrumbItem.LastWriteTime
    }
  }

  Start-Sleep -Seconds $PollSeconds
}
