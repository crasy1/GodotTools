dotnet clean --configuration Release
dotnet build --configuration Release
dotnet pack --configuration Release
dotnet nuget delete GodotTools 0.0.1 --non-interactive --source "local"
dotnet nuget push "GodotTools\bin\Release\GodotTools.*.nupkg" --source "local"

@REM dotnet nuget push "GodotTools\bin\Release\GodotTools.*.nupkg" --source "github"