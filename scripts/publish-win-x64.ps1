#!/usr/bin/env pwsh
# Publishes the BastionBuilder WPF app as a self-contained single-file executable for Windows x64.
# Output is written to: src/BastionBuilder.App/bin/Release/net8.0-windows/win-x64/publish/

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSScriptRoot

dotnet publish "$repoRoot/src/BastionBuilder.App" `
    -c Release `
    -r win-x64 `
    /p:PublishSingleFile=true `
    /p:SelfContained=true `
    /p:PublishReadyToRun=true

Write-Host ""
Write-Host "Publish complete. Output:" -ForegroundColor Green
Write-Host "  src/BastionBuilder.App/bin/Release/net8.0-windows/win-x64/publish/BastionBuilder.exe" -ForegroundColor Cyan
