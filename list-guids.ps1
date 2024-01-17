function AsGuid([string] $guid) { return [System.Guid]::Parse($guid) }

$guidFileGroups = @{}
$knownGuids = @(
    "2150E333-8FDC-42A3-9474-1A3956D46DE8",
    "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC",
    "9A19103F-16F7-4668-BE54-9A1E7A4F7556"
) | ForEach-Object { AsGuid $_ }

$skippedGuids = `
    Get-Content -Path ".template.config/template.json" | `
    ConvertFrom-Json | `
    Select-Object -ExpandProperty "guids" | `
    ForEach-Object { AsGuid $_ }

function Find-GuidsInFileAndUpdateHashtable {
    param (
        [string]$filePath
    )
    
    $content = Get-Content -Path $filePath -Raw
    if ($null -eq $content) { return }
    $guids = [regex]::Matches($content, '(?i)\b[A-F0-9]{8}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{4}-[A-F0-9]{12}\b')
    if ($guids -eq $null -or $guids.Count -eq 0) { return }

    $guids = $guids.Value | ForEach-Object { AsGuid $_ } | Select-Object -Unique

    foreach ($guid in $guids) {
        if (-not $guidFileGroups.ContainsKey($guid)) {
            $guidFileGroups[$guid] = @()
        }
        $guidFileGroups[$guid] += @($filePath)
    }
}

$folderPath = "$PSscriptRoot\content"

Push-Location $folderPath
try {
    $textFiles = Get-ChildItem -Include "*" -File -Recurse
    $textFiles = $textFiles | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }

    foreach ($textFile in $textFiles) {
        Write-Host "Scanning: $textFile" -ForegroundColor DarkGray
        Find-GuidsInFileAndUpdateHashtable -filePath $textFile.FullName
    }

    foreach ($guid in $skippedGuids) {
        if (-not $guidFileGroups.ContainsKey($guid)) {
            Write-Host "`n>>> $guid" -ForegroundColor Yellow
            Write-Host "  * excluded, but never used"
        }
    }

    foreach ($guid in $guidFileGroups.Keys | Sort-Object) {
        if ($guid -in $knownGuids -or $guid -in $skippedGuids) {
            continue
        }
        $filePaths = $guidFileGroups[$guid] | Sort-Object
        Write-Host "`n>>> $guid" -ForegroundColor Green
        foreach ($filePath in $filePaths) {
            $filePath = $filePath | Resolve-Path -Relative
            Write-Host "  * $filePath"
        }
    }

    Write-Host "`n----"
} finally {
    Pop-Location
}

