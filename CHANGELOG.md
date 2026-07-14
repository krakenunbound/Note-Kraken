# Changelog

## 1.1.0 — 2026-07-13

- Replaced the blue Kraken icon and styling with a distinct ivory, graphite, amber, and plum identity.
- Added redo, delete, complete clipboard menus, and state-aware edit commands.
- Added find-next, search direction, whole-word matching, wrap-around control, and safer replace behavior.
- Added new-window, time/date insertion, font selection, zoom, status-bar control, page setup, and printing.
- Added encoding detection/preservation and selectable output encoding.
- Added line-ending detection/preservation and selectable CRLF, LF, or CR output.
- Fixed drag-and-drop replacing an unsaved document without prompting.
- Fixed files opened through Explorer or the command line being incorrectly marked modified when the window first appeared.
- Fixed Save As losing the previous document path when a save failed.
- Improved go-to-line validation and disabled it while word wrap is active.
- Added a self-contained Windows build so the installed editor does not require a separate .NET runtime.
- Reworked installer theming, shortcuts, uninstall cleanup, and Windows Default Apps/Open With registration.
- Added reproducible UI screenshot and signed-release tooling.
