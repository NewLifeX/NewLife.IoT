#!/usr/bin/env pwsh
# 运行时消息英文检查：代码中的字符串字面量不得包含中文（元数据特性除外）
param(
    [String[]]$Dir = @("NewLife.IoT", "NewLife.IoTSerial", "NewLife.IoTSocket", "NewLife.IoTDatabase"),
    [Switch]$List,
    [String]$Csv
)

$ErrorActionPreference = "Stop"
$hits = @()

foreach ($d in $Dir) {
    if (-not (Test-Path $d)) { continue }

    $files = Get-ChildItem -Path $d -Recurse -Filter *.cs |
        Where-Object { $_.FullName -notmatch '[/\\](obj|bin)[/\\]' }

    foreach ($file in $files) {
        $lineNo = 0
        foreach ($line in Get-Content -LiteralPath $file.FullName -Encoding UTF8) {
            $lineNo++
            $text = $line.Trim()
            if ($text.StartsWith("//") -or $text.StartsWith("///") -or $text.StartsWith("*")) { continue }
            if ($line -notmatch '[\u4e00-\u9fff]') { continue }
            # 普通元数据特性按约定保留源码语言；本地化特性参数必须使用英文键
            if ($text -match '^\s*\[\s*[A-Za-z_]' -and $text -notmatch '^\s*\[\s*Localized(Description|DisplayName)\s*\(') { continue }
            # 仅检查字符串字面量，中文注释与中文标识符不在本门禁范围
            $literal = [regex]::Match($line, '"(?:[^"\\]|\\.)*"')
            if (-not $literal.Success) { continue }
            if ($literal.Value -match '[\u4e00-\u9fff]') {
                $hits += [pscustomobject]@{ File = $file.FullName; Line = $lineNo; Text = $text }
            }
        }
    }
}

if ($Csv) {
    $hits | Export-Csv -Path $Csv -NoTypeInformation -Encoding UTF8
    Write-Host "已导出 $($hits.Count) 条到 $Csv"
}
if ($List) {
    foreach ($hit in $hits) { Write-Host "$($hit.File):$($hit.Line): $($hit.Text)" }
}

if ($hits.Count -gt 0) {
    foreach ($hit in $hits) { Write-Host "[ENG001] $($hit.File):$($hit.Line): $($hit.Text)" }
    Write-Host "发现 $($hits.Count) 条中文运行时消息"
    exit 1
}

Write-Host "运行时消息英文检查通过"
