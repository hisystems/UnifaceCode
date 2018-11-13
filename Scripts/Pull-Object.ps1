# Pull one Uniface object to the local directory
param
(
   [Parameter(Mandatory=$true,HelpMessage="Type/Library/ObjectName")]
   [string]$objectId    
)

$configFile = "./uniface-config.json"
$config = Get-Content $configFile | ConvertFrom-Json

Write-Host "Pulling object '$objectId' from '$($config.database.name)' to '$($config.database.codeRootDir)'"

dotnet ./bin/UnifacePullObject.dll -d $config.database.connectionString --codeRootDir="$($config.database.codeRootDir)" --objectId $objectId
