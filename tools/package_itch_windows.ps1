param(
    [string]$GodotPath = "C:\Users\ryans\AppData\Roaming\Godot-Manager\versions\Godot_v4.6-stable_mono_win64\Godot_v4.6-stable_mono_win64.exe",
    [string]$Preset = "Windows Desktop"
)

$ErrorActionPreference = "Stop"

$ProjectRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$BuildDir = Join-Path $ProjectRoot "build\windows"
$ZipPath = Join-Path $ProjectRoot "build\Pongtime-windows.zip"
$ExePath = Join-Path $BuildDir "Pongtime.exe"

if (-not (Test-Path -LiteralPath $GodotPath)) {
    throw "Godot executable not found at '$GodotPath'. Pass -GodotPath with the path to Godot 4.6 Mono."
}

if (Test-Path -LiteralPath $BuildDir) {
    Remove-Item -LiteralPath $BuildDir -Recurse -Force
}

New-Item -ItemType Directory -Path $BuildDir -Force | Out-Null

$GodotArgs = "--headless --path `"$ProjectRoot`" --export-release `"$Preset`" `"$ExePath`""
$GodotProcess = Start-Process -FilePath $GodotPath -ArgumentList $GodotArgs -Wait -PassThru -NoNewWindow

if (-not (Test-Path -LiteralPath $ExePath)) {
    throw "Export did not create '$ExePath'."
}

$DataDir = Get-ChildItem -LiteralPath $BuildDir -Directory -Filter "data_Pongtime_windows_x86_64" -ErrorAction SilentlyContinue | Select-Object -First 1
if ($DataDir -and -not (Test-Path -LiteralPath (Join-Path $DataDir.FullName "Pongtime.dll"))) {
    throw "The .NET data directory exists, but Pongtime.dll is missing from it."
}

if (Test-Path -LiteralPath $ZipPath) {
    Remove-Item -LiteralPath $ZipPath -Force
}

Compress-Archive -Path (Join-Path $BuildDir "*") -DestinationPath $ZipPath -Force
Write-Host "Created itch upload: $ZipPath"
Write-Host "Upload the zip file to itch, not Pongtime.exe by itself."
