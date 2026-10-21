#!/usr/bin/env pwsh
# 运行时消息词表校验：异常、结果、翻译入口与日志模板必须登记到基线词表
param(
    [String[]]$Dir = @("NewLife.IoT", "NewLife.IoTSerial", "NewLife.IoTSocket", "NewLife.IoTDatabase"),
    [String]$LocaleDir = "Locales",
    [String]$Baseline = "zh-Hans.yml"
)

$ErrorActionPreference = "Stop"

# 脚本位于 tools，仓库根目录在其上一级；相对路径一律按仓库根解析
$root = Split-Path -Parent $PSScriptRoot
$Dir = @($Dir | ForEach-Object { if ([IO.Path]::IsPathRooted($_)) { $_ } else { Join-Path $root $_ } })
if (![IO.Path]::IsPathRooted($LocaleDir)) { $LocaleDir = Join-Path $root $LocaleDir }

$baselinePath = Join-Path $LocaleDir $Baseline
if (-not (Test-Path $baselinePath)) {
    Write-Host "[RUN000] 基线词表不存在：$baselinePath"
    exit 1
}

$keys = [System.Collections.Generic.HashSet[String]]::new([StringComparer]::Ordinal)
foreach ($line in Get-Content -LiteralPath $baselinePath -Encoding UTF8) {
    $text = $line.Trim()
    if ($text -eq "" -or $text.StartsWith("#")) { continue }
    if ($text -match '^"((?:[^"\\]|\\.)*)"\s*:\s*"(?:[^"\\]|\\.)*"\s*$') {
        [void]$keys.Add($matches[1])
    }
}

# 运行时消息入口：异常、结果失败、翻译快捷入口与日志方法
$callPattern = '(?s)\b(?:throw\s+new|\.Fail|WriteLog|WritePrefixed|InfoPrefixed|ErrorPrefixed|WarnPrefixed|DebugPrefixed|Log\??\.(?:Info|Warn|Error|Debug|Fatal)|XTrace\.Write(?:Line)?|L\.T|Localization\.Translate(?:ForLog)?|LocalizedDisplayName|LocalizedDescription)\b[^;]{0,400}'
$literalPattern = '(?<prefix>\$@?|@\$?)?"(?<lit>(?:[^"\\]|\\.)*)"'

$hits = @()
foreach ($d in $Dir) {
    if (-not (Test-Path $d)) { continue }

    $files = Get-ChildItem -Path $d -Recurse -Filter *.cs |
        Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' }

    foreach ($file in $files) {
        $lines = Get-Content -LiteralPath $file.FullName -Encoding UTF8 |
            ForEach-Object {
                $text = $_.Trim()
                if ($text.StartsWith("//") -or $text.StartsWith("///") -or $text.StartsWith("*")) { "" } else { $_ }
            }
        $raw = $lines -join "`n"

        foreach ($call in [regex]::Matches($raw, $callPattern)) {
            $literal = [regex]::Match($call.Value, $literalPattern)
            if (-not $literal.Success) { continue }

            $message = $literal.Groups['lit'].Value
            # 跳过纯格式串（如 {0,16}\t{1,-14}\t{2}）与非自然语言模板
            if ($message -notmatch '\s' -or $message -notmatch '[A-Za-z]') { continue }

            $line = $raw.Substring(0, $call.Index).Split("`n").Count
            if ($literal.Groups['prefix'].Success) {
                $hits += [pscustomobject]@{
                    Code    = "RUN002"
                    File    = $file.FullName
                    Line    = $line
                    Message = "运行时消息不得使用字符串插值：$message"
                }
            }
            elseif ($message -match '[\u4e00-\u9fff]') {
                $hits += [pscustomobject]@{
                    Code    = "RUN003"
                    File    = $file.FullName
                    Line    = $line
                    Message = "运行时消息必须使用英文源文：$message"
                }
            }
            elseif (-not $keys.Contains($message)) {
                $hits += [pscustomobject]@{
                    Code    = "RUN001"
                    File    = $file.FullName
                    Line    = $line
                    Message = "运行时消息缺少词表键：$message"
                }
            }
        }
    }
}

if ($hits.Count -gt 0) {
    foreach ($hit in $hits) {
        Write-Host "[$($hit.Code)] $($hit.File):$($hit.Line) $($hit.Message)"
    }
    Write-Host "发现 $($hits.Count) 条未登记的运行时消息"
    exit 1
}

Write-Host "运行时消息词表校验通过"
exit 0
