Write-Host "Executing dotnet pack"

$Version = git describe --tags
& dotnet @("pack", "src/GitEzTag/GitEzTag.csproj", "--force", "-c", "Release", "-p:Version=$Version")