dotnet publish -c Release
[string]$sourceDirectory  = ".\bin\Release\netcoreapp2.1\publish"
[string]$destinationDirectory = "C:\filmLib"

Write-Host "Deleting previous version..."
Remove-Item $destinationDirectory -Force -Recurse
Write-Host "Deploying the new version ..."
Copy-Item $sourceDirectory $destinationDirectory -Recurse -Force
Write-Host "Done."
