[CmdletBinding()]
param(
    [Parameter()]
    [string]$CertificateThumbprint = $env:NOTEKRAKEN_SIGNING_THUMBPRINT,

    [Parameter()]
    [string]$TimestampUrl = "http://timestamp.digicert.com"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$ProjectRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
$ReleaseBuild = Join-Path $ProjectRoot "release_build"
$ReleaseInstaller = Join-Path $ProjectRoot "release_installer"
$InstallerProject = Join-Path $ProjectRoot "src\Installer\Installer.csproj"
$SmokeTests = Join-Path $ProjectRoot "tests\NoteKraken.SmokeTests\NoteKraken.SmokeTests.csproj"

if ([string]::IsNullOrWhiteSpace($CertificateThumbprint)) {
    $certificate = Get-ChildItem Cert:\CurrentUser\My |
        Where-Object {
            $_.HasPrivateKey -and
            $_.NotAfter -gt (Get-Date) -and
            ($_.EnhancedKeyUsageList.ObjectId -contains "1.3.6.1.5.5.7.3.3")
        } |
        Sort-Object NotAfter -Descending |
        Select-Object -First 1

    if (-not $certificate) {
        throw "No Current User code-signing certificate with a private key was found."
    }
    $CertificateThumbprint = $certificate.Thumbprint
}

$certificatePath = "Cert:\CurrentUser\My\$CertificateThumbprint"
if (-not (Test-Path -LiteralPath $certificatePath)) {
    throw "The requested code-signing certificate was not found: $CertificateThumbprint"
}

$SignTool = Get-ChildItem "C:\Program Files (x86)\Windows Kits\10\bin" -Filter signtool.exe -Recurse -File |
    Where-Object { $_.FullName -match "\\x64\\signtool\.exe$" } |
    Sort-Object FullName -Descending |
    Select-Object -First 1 -ExpandProperty FullName

if (-not $SignTool) {
    throw "Windows SDK SignTool was not found."
}

function Remove-ReleaseDirectory {
    param([Parameter(Mandatory)][string]$Path)

    $resolved = [System.IO.Path]::GetFullPath($Path)
    if (-not $resolved.StartsWith($ProjectRoot + "\", [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to remove a path outside the project: $resolved"
    }
    if (Test-Path -LiteralPath $resolved) {
        Remove-Item -LiteralPath $resolved -Recurse -Force
    }
}

dotnet run --project $SmokeTests -c Release
if ($LASTEXITCODE -ne 0) { throw "Smoke tests failed." }

Remove-ReleaseDirectory $ReleaseBuild
Remove-ReleaseDirectory $ReleaseInstaller

dotnet publish (Join-Path $ProjectRoot "NoteKraken.csproj") -c Release -r win-x64 --self-contained true -o $ReleaseBuild
if ($LASTEXITCODE -ne 0) { throw "Editor publish failed." }

$Editor = Join-Path $ReleaseBuild "NoteKraken.exe"
& $SignTool sign /sha1 $CertificateThumbprint /s My /fd SHA256 /tr $TimestampUrl /td SHA256 /d "Note Kraken 1.1.0" $Editor
if ($LASTEXITCODE -ne 0) { throw "Editor signing failed." }

Remove-ReleaseDirectory (Join-Path $ProjectRoot "src\Installer\bin")
Remove-ReleaseDirectory (Join-Path $ProjectRoot "src\Installer\obj")
dotnet publish $InstallerProject -c Release -r win-x64 --self-contained true -o $ReleaseInstaller
if ($LASTEXITCODE -ne 0) { throw "Installer publish failed." }

$RawInstaller = Join-Path $ReleaseInstaller "NoteKraken_Setup.exe"
$Installer = Join-Path $ReleaseInstaller "NoteKraken_Setup_1.1.0.exe"
Move-Item -LiteralPath $RawInstaller -Destination $Installer -Force

& $SignTool sign /sha1 $CertificateThumbprint /s My /fd SHA256 /tr $TimestampUrl /td SHA256 /d "Note Kraken 1.1.0 Setup" $Installer
if ($LASTEXITCODE -ne 0) { throw "Installer signing failed." }

& $SignTool verify /pa /all /tw $Editor
if ($LASTEXITCODE -ne 0) { throw "Editor signature verification failed." }
& $SignTool verify /pa /all /tw $Installer
if ($LASTEXITCODE -ne 0) { throw "Installer signature verification failed." }

$checksumLines = Get-FileHash -Algorithm SHA256 -LiteralPath $Editor, $Installer |
    ForEach-Object { "{0}  {1}" -f $_.Hash, (Split-Path $_.Path -Leaf) }
$checksumLines | Set-Content -LiteralPath (Join-Path $ReleaseInstaller "SHA256SUMS.txt") -Encoding ascii

Write-Host "Signed release created successfully."
Write-Host $Editor
Write-Host $Installer
