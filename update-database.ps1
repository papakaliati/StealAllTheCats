# Usage: ./update-database.ps1 [project] [startupProject]

param (
    [string]$project = "./src/StealAllTheCats.csproj"  # path to your EF Core project
)

Write-Host "Running database update..."
dotnet ef database update --project $project