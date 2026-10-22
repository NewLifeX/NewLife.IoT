#!/usr/bin/env pwsh
# 多语言词表校验：语言清单、键集合、占位符、孤儿键、同形漏译
param(
    [String]$LocaleDir = "Locales",
    [String]$Baseline = "zh-Hans.yml",
    [String]$SourceDir = ".",
    [Switch]$SkipOrphan,
    [Switch]$SkipSameAsSource
)

$ErrorActionPreference = "Stop"
$script:errors = 0

# 脚本位于 tools，仓库根目录在其上一级；相对路径一律按仓库根解析
$root = Split-Path -Parent $PSScriptRoot
if (![IO.Path]::IsPathRooted($LocaleDir)) { $LocaleDir = Join-Path $root $LocaleDir }
if (![IO.Path]::IsPathRooted($SourceDir)) { $SourceDir = Join-Path $root $SourceDir }

$files = @(Get-ChildItem -Path $LocaleDir -Filter *.yml | Sort-Object Name)
if ($files.Count -eq 0) {
    Write-Host "[LOC000] 未找到词表目录或文件：$LocaleDir"
    exit 1
}

function Read-Locale([String]$path) {
    $map = [ordered]@{}
    $lineNo = 0
    foreach ($line in Get-Content -LiteralPath $path -Encoding UTF8) {
        $lineNo++
        $text = $line.Trim()
        if ($text -eq "" -or $text.StartsWith("#")) { continue }
        if ($text -notmatch '^"((?:[^"\\]|\\.)*)"\s*:\s*"((?:[^"\\]|\\.)*)"\s*$') {
            Write-Host "[LOC001] $([IO.Path]::GetFileName($path)):$lineNo 行格式错误"
            $script:errors++
            continue
        }
        $key = $matches[1]
        if ($map.Contains($key)) {
            Write-Host "[LOC002] $([IO.Path]::GetFileName($path)):$lineNo 重复键：$key"
            $script:errors++
            continue
        }
        $map[$key] = $matches[2]
    }
    return $map
}

function Get-Places([String]$text) {
    @([regex]::Matches($text, '\{[0-9]+[^{}]*\}') | ForEach-Object { $_.Value } | Sort-Object)
}

$tables = @{}
foreach ($f in $files) { $tables[$f.Name] = Read-Locale $f.FullName }

$baselineName = [IO.Path]::GetFileName($Baseline)
if (-not $tables.ContainsKey($baselineName)) {
    Write-Host "[LOC000] 基线词表不存在：$baselineName"
    exit 1
}
$base = $tables[$baselineName]

# 语言清单与 locales.json 对齐
$manifestPath = Join-Path $root "locales.json"
if (Test-Path $manifestPath) {
    $manifest = Get-Content -LiteralPath $manifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
    $actual = @($files | ForEach-Object { $_.BaseName })
    foreach ($code in $manifest.locales) {
        if ($actual -notcontains $code) {
            Write-Host "[LOC006] locales.json 声明了 $code，但缺少对应词表文件"
            $script:errors++
        }
    }
    foreach ($code in $actual) {
        if ($manifest.locales -notcontains $code) {
            Write-Host "[LOC007] 词表文件 $code.yml 未登记到 locales.json"
            $script:errors++
        }
    }
}

# 键集合与占位符
foreach ($name in $tables.Keys) {
    if ($name -eq $baselineName) { continue }

    $table = $tables[$name]
    foreach ($key in $base.Keys) {
        if (-not $table.Contains($key)) {
            Write-Host "[LOC003] $name 缺少键：$key"
            $script:errors++
        }
    }
    foreach ($key in $table.Keys) {
        if (-not $base.Contains($key)) {
            Write-Host "[LOC005] $name 存在多余键：$key"
            $script:errors++
        }
    }
    foreach ($key in $base.Keys) {
        if (-not $table.Contains($key)) { continue }
        $expected = (Get-Places $base[$key]) -join ","
        $actual = (Get-Places $table[$key]) -join ","
        if ($expected -ne $actual) {
            Write-Host "[LOC004] $name 占位符不一致：$key（$expected != $actual）"
            $script:errors++
        }
    }
}

# 孤儿键：词表键必须能在库源码中找到
# 源文范围含 .cs 与 .csproj：程序集标题/描述等元数据写在 csproj 中，同样是词表源文
if (-not $SkipOrphan) {
    $code = @(Get-ChildItem -Path $SourceDir -Recurse -Include *.cs, *.csproj |
        Where-Object { $_.FullName -notmatch '[/\\](obj|bin|XUnitTest|Test|AotSmoke)[/\\]' } |
        ForEach-Object { Get-Content -LiteralPath $_.FullName -Raw -Encoding UTF8 }) -join "`n"

    foreach ($key in $base.Keys) {
        if (-not $code.Contains($key)) {
            Write-Host "[LOC008] 孤儿键（源码中未找到）：$key"
            $script:errors++
        }
    }
}

# 同形检测：译文与英文源文完全相同，通常是未翻译；
# 目标语言本就如此书写的技术词登记到 same-as-source.json，白名单失效同样报错。
if (-not $SkipSameAsSource) {
    $whitelistPath = Join-Path $LocaleDir "same-as-source.json"
    $whitelist = @{}
    if (Test-Path -LiteralPath $whitelistPath) {
        $manifest = Get-Content -LiteralPath $whitelistPath -Raw -Encoding UTF8 | ConvertFrom-Json
        foreach ($item in $manifest.entries.PSObject.Properties) {
            $whitelist[$item.Name] = @($item.Value)
        }
    }

    foreach ($name in $tables.Keys) {
        if ($name -eq $baselineName) { continue }

        $culture = [IO.Path]::GetFileNameWithoutExtension($name)
        $table = $tables[$name]
        $allowed = if ($whitelist.ContainsKey($culture)) { @($whitelist[$culture]) } else { @() }
        foreach ($key in $base.Keys) {
            if (-not $table.Contains($key)) { continue }
            if ($table[$key] -ne $key) { continue }
            if ($allowed -contains $key) { continue }

            Write-Host "[LOC009] ${culture} 译文与英文原文相同（疑为未翻译）：$key"
            $script:errors++
        }

        foreach ($key in $allowed) {
            if (-not $table.Contains($key) -or $table[$key] -ne $key) {
                Write-Host "[LOC010] ${culture} 同形白名单已失效（译文已翻译或键已删除）：$key"
                $script:errors++
            }
        }
    }
}

if ($script:errors -gt 0) {
    Write-Host "词表校验失败：$script:errors 项"
    exit 1
}

Write-Host "词表校验通过：$($files.Count) 种语言，$($base.Count) 个键"
