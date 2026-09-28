# Creates the "guildmanager" PostgreSQL database (if missing) and applies the
# schema. Needs PostgreSQL installed locally, with psql on the PATH.
# Usage (from any directory):  .\Logic\Database\setup-database.ps1
param(
    [string]$PgHost = "localhost",
    [int]$PgPort = 5432,
    [string]$PgUser = "postgres",
    [string]$PgPassword = "postgres",
    [string]$Database = "guildmanager"
)

$env:PGPASSWORD = $PgPassword

$exists = psql -h $PgHost -p $PgPort -U $PgUser -tAc "SELECT 1 FROM pg_database WHERE datname='$Database'"
if (-not $exists) {
    Write-Host "Creating database '$Database'..."
    psql -h $PgHost -p $PgPort -U $PgUser -c "CREATE DATABASE $Database;"
} else {
    Write-Host "Database '$Database' already exists."
}

Write-Host "Applying schema..."
psql -h $PgHost -p $PgPort -U $PgUser -d $Database -f (Join-Path $PSScriptRoot "00_run_all.sql")
Write-Host "Done."
