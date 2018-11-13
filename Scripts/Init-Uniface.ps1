# Initializes the config file.
param
(
    [string]$databaseName = "main",     # Reference name
    [string]$databaseConnectionString = "Server=localhost;Initial Catalog=DatabaseName;User Id=admin;Password=;",
    [string]$codeRootDir = "$env:homepath\source\UnifaceMain"
)

$config = @{
    database = @{
        name = $databaseName
        connectionString = $databaseConnectionString
        pulledVersion = 0
        codeRootDir = $codeRootDir
    }
}

$configFile = "./uniface-config.json"

Remove-Item $configFile
$config | ConvertTo-Json | Add-Content -Path $configFile

Write-Host "$configFile config initialised"