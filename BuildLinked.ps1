# Set Working Directory
Split-Path $MyInvocation.MyCommand.Path | Push-Location
[Environment]::CurrentDirectory = $PWD

Remove-Item "$env:RELOADEDIIMODS/Neptunia_DataStructs/*" -Force -Recurse
dotnet publish "./Neptunia_DataStructs.csproj" -c Release -o "$env:RELOADEDIIMODS/Neptunia_DataStructs" /p:OutputPath="./bin/Release" /p:ReloadedILLink="true"

# Restore Working Directory
Pop-Location