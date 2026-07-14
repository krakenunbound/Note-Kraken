# Note Kraken

Note Kraken is a fast, focused Windows text editor: no accounts, cloud sync, telemetry, AI, or plugin system—just the useful parts of classic Notepad.

![Note Kraken icon](https://raw.githubusercontent.com/krakenunbound/Note-Kraken/master/assets/note-kraken.png)

## Screenshots

### Editor

![Note Kraken editor showing encoding, line endings, zoom, and cursor position](https://raw.githubusercontent.com/krakenunbound/Note-Kraken/master/docs/screenshots/note-kraken-editor.png)

### Find and replace

![Note Kraken Find and Replace dialog](https://raw.githubusercontent.com/krakenunbound/Note-Kraken/master/docs/screenshots/note-kraken-find-replace.png)

### Installer and file associations

![Note Kraken installer with shortcut and file-association choices](https://raw.githubusercontent.com/krakenunbound/Note-Kraken/master/docs/screenshots/note-kraken-installer.png)

## Features

- Native Windows Forms application with quick startup
- Distinct warm graphite-and-amber theme that follows the Windows light/dark setting
- New, new window, open, save, and save as
- Unsaved-change protection when closing, opening, creating, or dropping another file
- Undo, redo, cut, copy, paste, delete, and select all
- Complete right-click editing menu with commands enabled only when applicable
- Find and replace with match case, whole word, direction, and wrap-around options
- Find next (`F3`), go to line, and insert time/date (`F5`)
- Word wrap and user-selectable editor font
- Zoom from 50% to 500%
- Page setup and printing
- Drag-and-drop file opening
- Status bar with cursor position, zoom, encoding, and line-ending style
- Preserves UTF-8, UTF-8 BOM, UTF-16, UTF-32, and Windows-1252 text when opening files
- Selectable UTF-8, UTF-8 BOM, UTF-16 LE, or Windows-1252 output
- Selectable Windows (CRLF), Unix (LF), or classic Mac (CR) line endings
- Self-contained 64-bit Windows build; no separate .NET runtime installation required

## Keyboard shortcuts

| Shortcut | Action |
|---|---|
| `Ctrl+N` | New file |
| `Ctrl+Shift+N` | New window |
| `Ctrl+O` | Open |
| `Ctrl+S` | Save |
| `Ctrl+Shift+S` | Save as |
| `Ctrl+P` | Print |
| `Ctrl+Z` / `Ctrl+Y` | Undo / redo |
| `Ctrl+X` / `Ctrl+C` / `Ctrl+V` | Cut / copy / paste |
| `Delete` | Delete selection |
| `Ctrl+F` / `F3` | Find / find next |
| `Ctrl+H` | Replace |
| `Ctrl+G` | Go to line (when word wrap is off) |
| `Ctrl+A` | Select all |
| `F5` | Insert time and date |
| `Ctrl++` / `Ctrl+-` / `Ctrl+0` | Zoom in / out / reset |

## Install

Run `NoteKraken_Setup_1.1.0.exe` from the `release_installer` folder.

The installer can create desktop and Start Menu shortcuts and register Note Kraken with Windows Default Apps/Open With for:

- `.txt`
- `.log`
- `.ini` and `.cfg`
- `.md`

Windows controls the final choice of default application and may ask for confirmation the first time a file type is opened.

Release binaries are Authenticode-signed and RFC 3161 timestamped. The current `Kraken Unbound` certificate is a machine-local development certificate, not a certificate issued by a public certification authority. It verifies as trusted on the creator's workstation, but other computers can still show an unknown-publisher or SmartScreen warning. See [SIGNING.md](SIGNING.md).

## Build from source

Requires the .NET 8 SDK on Windows.

```powershell
dotnet build .\NoteKraken.sln -c Release
dotnet publish .\NoteKraken.csproj -c Release -r win-x64 --self-contained true -o .\release_build
dotnet publish .\src\Installer\Installer.csproj -c Release -r win-x64 --self-contained true -o .\release_installer
```

The installer project embeds the already-published `release_build\NoteKraken.exe`, so publish the editor first.

For a reproducible signed build on a workstation that has an appropriate code-signing certificate:

```powershell
.\scripts\Build-SignedRelease.ps1 -CertificateThumbprint YOUR_CERTIFICATE_THUMBPRINT
```

## Philosophy

Note Kraken intentionally does not include AI, cloud services, collaboration, extensions, syntax intelligence, or other IDE features. Those belong in larger editors. Note Kraken exists to open, edit, print, and save plain text reliably.

## License

MIT License — Copyright © 2026 The Kraken (Kraken Unbound)
