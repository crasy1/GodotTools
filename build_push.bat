dotnet build --configuration Release
dotnet nuget push "GodotTools\bin\Release\GodotTools.*.nupkg" --source "local"
@REM dotnet nuget push "GodotTools\bin\Release\GodotTools.*.nupkg" --source "github"