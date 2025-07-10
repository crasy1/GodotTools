@echo off
chcp 65001
setlocal enabledelayedexpansion

set "proj_file=GodotTools\\GodotTools.csproj"
set "version="

if not exist "%proj_file%" (
    echo 错误: 文件 %proj_file% 不存在
    exit /b 1
)

for /f "tokens=3 delims=<>" %%a in ('type "%proj_file%" ^| findstr /i /c:"<Version>"') do (
    if not defined version set "version=%%a"
)

if defined version (
    echo 找到版本号: %version%
    setx PACKAGE_VERSION "%version%" /M >nul 2>&1
) else (
    echo 错误: 未找到 <Version> 标签
    exit /b 1
)

dotnet clean --configuration Release
dotnet build --configuration Release
dotnet pack --configuration Release
dotnet nuget delete GodotTools %version% --non-interactive --source "local"
dotnet nuget push "GodotTools\bin\Release\GodotTools.%version%.nupkg" --source "local"

dotnet nuget push "GodotTools\bin\Release\GodotTools.%version%.nupkg" --source "github"

endlocal