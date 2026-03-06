#!/usr/bin/env pwsh
# Publishes the BastionBuilder WPF app as a self-contained single-file executable for Windows x64.

$ErrorActionPreference = 'Stop'

$configuration = 'Release'
$runtime       = 'win-x64'
$repoRoot      = Split-Path -Parent $PSScriptRoot

dotnet publish "$repoRoot/src/BastionBuilder.App" `
    -c $configuration `
    -r $runtime `
    /p:PublishSingleFile=true `
    /p:SelfContained=true `
    /p:PublishReadyToRun=true

if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish failed with exit code $LASTEXITCODE"
    exit $LASTEXITCODE
}

$outputPath = "src/BastionBuilder.App/bin/$configuration/net8.0-windows/$runtime/publish/BastionBuilder.exe"
Write-Host ""
Write-Host "Publish complete. Output:" -ForegroundColor Green
Write-Host "  $outputPath" -ForegroundColor Cyan
