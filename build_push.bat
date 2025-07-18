@echo off
chcp 65001
setlocal enabledelayedexpansion

set "tools_file=GodotTools\\GodotTools.csproj"
set "tools_sg_file=SourceGenerators\\SourceGenerators\\SourceGenerators.csproj"
set "version="
set "sg_version="

if not exist "%tools_file%" (
    echo 错误: 文件 %tools_file% 不存在
    exit /b 1
)
if not exist "%tools_sg_file%" (
    echo 错误: 文件 %tools_sg_file% 不存在
    exit /b 1
)
for /f "tokens=3 delims=<>" %%a in ('type "%tools_file%" ^| findstr /i /c:"<Version>"') do (
    if not defined version set "version=%%a"
)
if defined version (
    echo 找到版本号: %version%
    setx PACKAGE_VERSION "%version%" /M >nul 2>&1
) else (
    echo 错误: 未找到 <Version> 标签
    exit /b 1
)

for /f "tokens=3 delims=<>" %%a in ('type "%tools_sg_file%" ^| findstr /i /c:"<Version>"') do (
    if not defined sg_version set "sg_version=%%a"
)
if defined sg_version (
    echo 找到版本号: %sg_version%
    setx SG_PACKAGE_VERSION "%sg_version%" /M >nul 2>&1
) else (
    echo 错误: 未找到 <Version> 标签
    exit /b 1
)


dotnet clean --configuration Release
dotnet build --configuration Release
dotnet pack --configuration Release
dotnet nuget delete GodotTools %version% --non-interactive --source "local"
dotnet nuget delete GodotTools.SourceGenerators %sg_version% --non-interactive --source "local"
dotnet nuget push "GodotTools\bin\Release\GodotTools.%version%.nupkg" --source "local"
dotnet nuget push "SourceGenerators\SourceGenerators\bin\Release\GodotTools.SourceGenerators.%sg_version%.nupkg" --source "local"

@REM dotnet nuget push "GodotTools\bin\Release\GodotTools.%version%.nupkg" --source "github"
@REM dotnet nuget push "GodotTools\bin\Release\GodotTools.SourceGenerators.%sg_version%.nupkg" --source "github"

endlocal