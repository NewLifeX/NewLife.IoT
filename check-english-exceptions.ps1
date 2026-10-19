#!/usr/bin/env pwsh
# 运行时消息英文检查：异常、日志与结果消息不得包含中文
param(
    [String[]]$Dir = @("NewLife.IoT", "NewLife.IoTSerial", "NewLife.IoSocket", "NewLife.IoTDatabase"),
    [Switch]$List,
    [String]$Csv
)

$ErrorActionPreference = "Stop"
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
            if ($line -notmatch '[\u4e00-\u9fff]') { continue }
            if ($line -match '(throw\s+new|\.Fail\(|WriteLog\(|XTrace\.Write(Line)?\()') {
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
