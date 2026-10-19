#!/usr/bin/env pwsh
# README 静态徽章指标同步：locales 语言数与 TFM 目标框架数
param(
    [Switch]$Check
)

$ErrorActionPreference = "Stop"

$metricsPath = Join-Path (Get-Location) "readme-metrics.json"
if (-not (Test-Path $metricsPath)) {
    Write-Host "[README000] 未找到 readme-metrics.json"
    exit 1
}

$metrics = Get-Content -LiteralPath $metricsPath -Raw -Encoding UTF8 | ConvertFrom-Json
$failed = 0
$updated = 0

foreach ($file in Get-ChildItem -Path . -Filter "Readme*.md") {
    $text = Get-Content -LiteralPath $file.FullName -Raw -Encoding UTF8
    $new = $text `
        -replace 'locales-\d+-2ea44f', "locales-$($metrics.locales)-2ea44f" `
        -replace 'TFM-\d+%20', "TFM-$($metrics.tfm)%20"

    if ($new -ne $text) {
        if ($Check) {
            Write-Host "[README001] $($file.Name) 徽章指标与 readme-metrics.json 不一致"
            $failed++
        }
        else {
            Set-Content -LiteralPath $file.FullName -Value $new -NoNewline -Encoding UTF8
            $updated++
            Write-Host "已更新 $($file.Name)"
        }
    }
}

if ($Check) {
    if ($failed -gt 0) {
        Write-Host "README 指标校验失败：$failed 个文件"
        exit 1
    }
    Write-Host "README 指标校验通过"
}
else {
    Write-Host "README 指标同步完成：更新 $updated 个文件"
}
