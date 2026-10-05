$ErrorActionPreference = 'Stop'

Write-Host '1) Starting PostgreSQL...'
docker compose up -d postgres

Write-Host '2) Installing/updating EF CLI...'
dotnet tool update --global dotnet-ef --version 9.0.10
if ($LASTEXITCODE -ne 0) { dotnet tool install --global dotnet-ef --version 9.0.10 }

Write-Host '3) Creating migration if it does not exist...'
$infra = Join-Path $PSScriptRoot '..\src\Approval.Infrastructure'
$api = Join-Path $PSScriptRoot '..\src\Approval.Api'
$migrations = Join-Path $infra 'Migrations'
if (-not (Test-Path $migrations)) {
  dotnet ef migrations add InitialCreate --project $infra --startup-project $api --output-dir Migrations
}

Write-Host '4) Updating database...'
dotnet ef database update --project $infra --startup-project $api
Write-Host 'Done.'
