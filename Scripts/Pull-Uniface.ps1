# Pulls all of the Uniface objects to the local directory

$configFile = "./uniface-config.json"
# $configFile = "C:\Users\twicks\source\UnifaceMain\uniface-config.json"
$config = Get-Content $configFile | ConvertFrom-Json

Write-Host "Pulling all objects from '$($config.database.name)' to '$($config.database.codeRootDir)'"

dotnet ./bin/UnifaceGetAllObjectIds.dll -d $config.database.connectionString | dotnet ./bin/UnifacePullObject.dll -d $config.database.connectionString --codeRootDir="$($config.database.codeRootDir)"