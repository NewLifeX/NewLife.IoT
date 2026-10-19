#!/usr/bin/env pwsh
# 日志词表校验：源码中的日志模板必须存在于词表，避免运行时回退英文
param(
    [String[]]$Dir = @("NewLife.IoT", "NewLife.IoTSerial", "NewLife.IoTSocket", "NewLife.IoTDatabase"),
    [String]$LocaleDir = "Locales",
    [String]$Baseline = "zh-Hans.yml"
)

$ErrorActionPreference = "Stop"

# 读取基线词表键
$baselinePath = Join-Path $LocaleDir $Baseline
if (-not (Test-Path $baselinePath)) {
    Write-Host "[LOG000] 基线词表不存在：$baselinePath"
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

$callPattern = '\b(?:WriteLog|InfoPrefixed|ErrorPrefixed|WarnPrefixed|DebugPrefixed|XTrace\.WriteLine)\s*\('
$stringPattern = '"((?:[^"\\]|\\.)*)"'
$hits = @()

foreach ($d in $Dir) {
    if (-not (Test-Path $d)) { continue }

    $files = Get-ChildItem -Path $d -Recurse -Filter *.cs |
        Where-Object { $_.FullName -notmatch '\\(obj|bin)\\' }

    foreach ($file in $files) {
        $lineNo = 0
        foreach ($line in Get-Content -LiteralPath $file.FullName -Encoding UTF8) {
            $lineNo++
            $text = $line.Trim()
            if ($text.StartsWith("//") -or $text.StartsWith("///") -or $text.StartsWith("*")) { continue }

            $call = [regex]::Match($line, $callPattern)
            if (-not $call.Success) { continue }

            $rest = $line.Substring($call.Index + $call.Length)
            $literal = [regex]::Match($rest, $stringPattern)
            if (-not $literal.Success) { continue }

            $message = $literal.Groups[1].Value
            # 跳过纯格式串（如 {0,16}\t{1,-14}\t{2}）与非自然语言模板
            if ($message -notmatch ' ') { continue }
            if ($message -notmatch '[A-Za-z]') { continue }
            if (-not $keys.Contains($message)) {
                $hits += [pscustomobject]@{
                    File    = $file.FullName
                    Line    = $lineNo
                    Message = $message
                }
            }
        }
    }
}

if ($hits.Count -gt 0) {
    foreach ($hit in $hits) {
        Write-Host "[LOG001] $($hit.File):$($hit.Line) 日志模板缺少词表键：$($hit.Message)"
    }
    Write-Host "发现 $($hits.Count) 条未登记的日志模板"
    exit 1
}

Write-Host "日志词表校验通过"
exit 0
