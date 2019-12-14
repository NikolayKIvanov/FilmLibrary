dotnet publish 'C:\Internship\Work\Interns2019\Yordan\FilmLibrary\FilmLibraryProject\FilmLibrary\FilmLibrary.csproj' -c Release
[string]$sourceDirectory  = "C:\Internship\Work\Interns2019\Yordan\FilmLibrary\FilmLibraryProject\FilmLibrary\bin\Release\netcoreapp2.1\publish"
[string]$destinationDirectory = "C:\filmLib"

iisreset /stop

Write-Host "Deleting previous version..."
Remove-Item $destinationDirectory -Force -Recurse
Write-Host "Deploying the new version ..."
Copy-Item $sourceDirectory $destinationDirectory -Recurse -Force
Write-Host "Done."

iisreset /start