# Signing and Windows SmartScreen

## Current 1.1.0 release

The Windows executables are signed with Authenticode using SHA-256 and an RFC 3161 DigiCert timestamp.

- Subject: `CN=Kraken Unbound`
- Certificate thumbprint: `6BB19C61065ED41B37D24E5EE71956F6BF9D0A03`
- Certificate expiration: July 13, 2031
- Private key: non-exportable and stored in the creator's Windows Current User certificate store

This is a self-signed development certificate. It is trusted in the creator's Current User Trusted Root and Trusted Publishers stores, so Windows validates the release locally. It is **not publicly trusted** and does not remove warnings on other computers.

Do not tell users to weaken SmartScreen or blindly install a self-signed root certificate. A public release should move to one of these options:

1. Publish through the Microsoft Store. Store-distributed apps are signed by Microsoft and avoid SmartScreen download warnings.
2. Use [Azure Artifact Signing](https://learn.microsoft.com/azure/artifact-signing/overview) for managed public-trust Authenticode signing.
3. Use an OV or EV code-signing certificate issued by a certification authority in the Microsoft Trusted Root Program.

SmartScreen also evaluates each downloaded file's reputation. Microsoft documents the current behavior in [SmartScreen reputation for Windows app developers](https://learn.microsoft.com/windows/apps/package-and-deploy/smartscreen-reputation).

## Building and signing

The signed-release script requires:

- Windows and the .NET 8 SDK
- Windows SDK SignTool
- A Current User certificate with the Code Signing enhanced key usage and an accessible private key

Run:

```powershell
.\scripts\Build-SignedRelease.ps1 -CertificateThumbprint YOUR_CERTIFICATE_THUMBPRINT
```

The script deliberately signs in this order:

1. Build the self-contained editor.
2. Sign and timestamp the editor.
3. Build the installer so it embeds the signed editor.
4. Sign and timestamp the installer.
5. Verify both signatures and create `SHA256SUMS.txt`.

Never commit or upload a `.pfx`, private key, password, or cloud signing credential.
