#!/usr/bin/env pwsh
# NuGet 包消费验证：打包四个项目，用本地源创建临时消费者，验证多语言、驱动工厂与参数绑定
param(
    [String]$Version = "",
    [Switch]$Keep
)

$ErrorActionPreference = "Stop"

if ([String]::IsNullOrWhiteSpace($Version)) {
    $Version = "9.9.9-local." + [DateTime]::Now.ToString("yyyyMMddHHmmss")
}

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$work = Join-Path ([IO.Path]::GetTempPath()) ("iot-package-" + [Guid]::NewGuid().ToString("N").Substring(0, 8))
$packages = Join-Path $work "packages"
$app = Join-Path $work "app"

New-Item -ItemType Directory -Path $packages -Force | Out-Null

try {
    $projects = @(
        "NewLife.IoT\NewLife.IoT.csproj",
        "NewLife.IoTSerial\NewLife.IoTSerial.csproj",
        "NewLife.IoTSocket\NewLife.IoTSocket.csproj",
        "NewLife.IoTDatabase\NewLife.IoTDatabase.csproj"
    )

    foreach ($project in $projects) {
        Write-Host "打包 $project ..."
        dotnet pack (Join-Path $root $project) -c Release -o $packages -p:Version=$Version -p:PackageVersion=$Version -v q
        if ($LASTEXITCODE -ne 0) { throw "打包失败：$project" }
    }

    $nugetConfig = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="local" value="$packages" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
</configuration>
"@
    Set-Content -LiteralPath (Join-Path $work "NuGet.config") -Value $nugetConfig -Encoding UTF8

    New-Item -ItemType Directory -Path $app -Force | Out-Null
    $csproj = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net10.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="NewLife.IoT" Version="$Version" />
    <PackageReference Include="NewLife.IoTSerial" Version="$Version" />
    <PackageReference Include="NewLife.IoTSocket" Version="$Version" />
    <PackageReference Include="NewLife.IoTDatabase" Version="$Version" />
  </ItemGroup>
</Project>
"@
    Set-Content -LiteralPath (Join-Path $app "PackageSmoke.csproj") -Value $csproj -Encoding UTF8

    $program = @"
using System.Globalization;
using NewLife;
using NewLife.IoT.Drivers;
using NewLife.IoTSerial.Drivers;

var saved = Localization.Culture;
try
{
    Localization.Culture = new CultureInfo("zh-CN");
    var text = Localization.Translate("Parameter cannot be null.");
    if (text != "参数不能为空。")
    {
        Console.WriteLine(`"LOCALIZATION FAILED: `" + text);
        return 1;
    }
}
finally
{
    Localization.Culture = saved;
}

DriverFactory.Register<IoTSerialDriver>("IoTSerial");
var driver = DriverFactory.Create("IoTSerial", "package-smoke") as IoTSerialDriver;
if (driver == null)
{
    Console.WriteLine("FACTORY FAILED");
    return 2;
}

try
{
    var parameter = DriverFactory.CreateParameter("IoTSerial", `"{\`"PortName\`":\`"COM1\`",\`"Baudrate\`":9600}`");
    if (parameter is not IoTSerialParameter p || p.PortName != "COM1" || p.Baudrate != 9600)
    {
        Console.WriteLine("PARAMETER FAILED");
        return 3;
    }
}
finally
{
    driver.Dispose();
}

Console.WriteLine("PACKAGE SMOKE PASS");
return 0;
"@
    Set-Content -LiteralPath (Join-Path $app "Program.cs") -Value $program -Encoding UTF8

    Push-Location $work
    try {
        dotnet run --project $app -c Release -v q
        if ($LASTEXITCODE -ne 0) { throw "消费者运行失败，退出码 $LASTEXITCODE" }
    }
    finally {
        Pop-Location
    }

    Write-Host "包消费验证通过：$Version"
}
finally {
    if (-not $Keep) {
        Remove-Item -Recurse -Force $work -ErrorAction SilentlyContinue
    }
    else {
        Write-Host "保留临时目录：$work"
    }
}
