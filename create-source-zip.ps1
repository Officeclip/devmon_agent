<#
.SYNOPSIS
  Create a source-focused ZIP without changing the original project tree.
.EXAMPLE
  .\create-source-zip.ps1
.EXAMPLE
  .\create-source-zip.ps1 -SourceRoot 'C:\OfficeClipNew\OpenSource\devmon_agent' -OutputZip 'C:\Temp\devmon-source.zip'
#>
[CmdletBinding()]
param(
    [string]$SourceRoot = 'C:\OfficeClipNew\OpenSource\devmon_agent',
    [string]$OutputZip = '',
    [switch]$IncludeDatabaseFiles
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

$SourceRoot = [System.IO.Path]::GetFullPath($SourceRoot).TrimEnd('\', '/')
if (-not (Test-Path -LiteralPath $SourceRoot -PathType Container)) {
    throw "Source folder does not exist: $SourceRoot"
}
if (-not $OutputZip) {
    $OutputZip = Join-Path (Split-Path -Parent $SourceRoot) ((Split-Path -Leaf $SourceRoot) + '-source.zip')
}
$OutputZip = [System.IO.Path]::GetFullPath($OutputZip)
if ($OutputZip.StartsWith($SourceRoot + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw 'Place OutputZip outside SourceRoot to avoid recursive/self inclusion.'
}

# Exclude generated artifacts and downloaded dependencies, regardless of nesting depth.
$excludeDirs = @(
    '.git', '.svn', '.hg', '.vs', '.idea', '.vscode',
    'bin', 'obj', 'packages', 'node_modules', 'bower_components',
    'TestResults', 'artifacts', 'publish', 'published', 'app.publish',
    'Debug', 'Release', 'x64', 'x86', 'ipch', 'DebugPublic',
    '_ReSharper.Caches', '.nuget', '.cache', 'coverage', '.nyc_output',
    'dist', 'build', 'logs', 'log', 'temp', 'tmp',
    'Backup', '_UpgradeReport_Files'
)
$excludeFiles = @(
    'Thumbs.db', 'desktop.ini', '.DS_Store', 'UpgradeLog.htm',
    'UpgradeLog.xml', 'project.assets.json', 'project.nuget.cache',
    'packages.lock.json'
)
$excludeExtensions = @(
    '.dll', '.exe', '.pdb', '.mdb', '.ilk', '.obj', '.lib', '.exp',
    '.cache', '.tmp', '.temp', '.log', '.tlog', '.lastbuildstate',
    '.user', '.suo', '.sln.docstates', '.pidb', '.dbmdl',
    '.zip', '.7z', '.rar', '.nupkg', '.snupkg', '.vsix', '.msi',
    '.msm', '.cab', '.iso', '.bak', '.binlog'
)
$excludedDirSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
$excludedFileSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
$excludedExtSet = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
foreach ($n in $excludeDirs) { [void]$excludedDirSet.Add($n) }
foreach ($n in $excludeFiles) { [void]$excludedFileSet.Add($n) }
foreach ($n in $excludeExtensions) { [void]$excludedExtSet.Add($n) }

$destDir = Split-Path -Parent $OutputZip
if (-not (Test-Path -LiteralPath $destDir)) {
    New-Item -ItemType Directory -Path $destDir -Force | Out-Null
}
if (Test-Path -LiteralPath $OutputZip) { Remove-Item -LiteralPath $OutputZip -Force }

$count = 0L
$bytes = 0L
$skipped = 0L
$rootName = Split-Path -Leaf $SourceRoot
$stream = [System.IO.File]::Open($OutputZip, [System.IO.FileMode]::CreateNew)
try {
    $archive = [System.IO.Compression.ZipArchive]::new($stream, [System.IO.Compression.ZipArchiveMode]::Create, $true)
    try {
        $pending = [System.Collections.Generic.Stack[string]]::new()
        $pending.Push($SourceRoot)
        while ($pending.Count -gt 0) {
            $folder = $pending.Pop()
            foreach ($entry in [System.IO.Directory]::EnumerateFileSystemEntries($folder)) {
                $name = [System.IO.Path]::GetFileName($entry)
                $attributes = [System.IO.File]::GetAttributes($entry)
                # Do not follow junctions/symlinks outside the source tree.
                if (($attributes -band [System.IO.FileAttributes]::ReparsePoint) -ne 0) {
                    $skipped++
                    continue
                }
                if (($attributes -band [System.IO.FileAttributes]::Directory) -ne 0) {
                    if ($excludedDirSet.Contains($name)) { $skipped++; continue }
                    $pending.Push($entry)
                    continue
                }
                $extension = [System.IO.Path]::GetExtension($name)
                if ($excludedFileSet.Contains($name) -or $excludedExtSet.Contains($extension) -or
                    $name -match '\.(?:csproj|vbproj|vcxproj|fsproj)\.user$' -or
                    $name -match '\.DotSettings\.user$') {
                    $skipped++
                    continue
                }
                if (-not $IncludeDatabaseFiles -and $extension -in @('.db', '.sqlite', '.sqlite3', '.mdf', '.ldf')) {
                    $skipped++
                    continue
                }
                $rel = $entry.Substring($SourceRoot.Length).TrimStart('\', '/') -replace '\\', '/'
                $zipEntryName = $rootName + '/' + $rel
                [void][System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile(
                    $archive, $entry, $zipEntryName,
                    [System.IO.Compression.CompressionLevel]::Optimal
                )
                $count++
                $bytes += ([System.IO.FileInfo]::new($entry)).Length
            }
        }
    }
    finally { $archive.Dispose() }
}
finally { $stream.Dispose() }

Write-Host "Created: $OutputZip"
Write-Host "Included files: $count"
Write-Host "Skipped items: $skipped"
Write-Host ('Source file size: {0:N2} MB' -f ($bytes / 1MB))
Write-Host ('ZIP size: {0:N2} MB' -f ((Get-Item -LiteralPath $OutputZip).Length / 1MB))
Write-Warning 'Review included config files for passwords, API keys, and other secrets before sharing.'
