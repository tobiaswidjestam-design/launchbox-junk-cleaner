# Junk Cleaner for LaunchBox

**By Aphex Studio Ryssby, 2026**

Repository: https://github.com/tobiaswidjestam-design/launchbox-junk-cleaner

Removes non-game entries (`.dll`, `.bat`, `.ico`, `.log`, `.md5`, `.txt`, `.nfo`, images ...) that
Automatic Import or a folder import pulled into your LaunchBox library.

Every candidate is shown in a review window with a checkbox, so nothing is deleted without you
seeing it first. The plugin works through the LaunchBox plugin API (`IDataManager.TryRemoveGame`),
so the data files are never edited by hand and LaunchBox does not have to be closed.

## Requirements

- LaunchBox for Windows **13.x or newer** (built against the LaunchBox 14.1 plugin SDK, .NET 10)
- Windows 10/11, x64

## Install

1. Close LaunchBox.
2. Copy the folder `JunkCleaner` (containing `JunkCleaner.Plugin.dll` and `JunkCleaner.Plugin.deps.json`)
   into `LaunchBox\Plugins\`.
3. Start LaunchBox and open **Tools -> Junk Cleaner - remove non-game entries**.

## Usage

- The window lists every entry whose `ApplicationPath` has a non-game extension, with title, file name
  and platform.
- All entries are ticked by default. Untick anything you want to keep, or use **Select all / Select none**.
- **Dry run (report only, change nothing)** shows what would be removed without touching the library -
  useful for a first look on a big collection.
- **Remove ticked (N)** removes the ticked entries and saves the library. A summary dialog follows.

## Be careful

Some file types in the list are legitimate games on specific systems:

| Extension | Can be a real game on |
|---|---|
| `.bat`, `.cmd` | DOS, ScummVM setups, Start-menu style launchers |
| `.png` | Pico-8 carts |
| `.json` | some MUGEN / ScummVM / frontend setups |
| `.exe`-adjacent helpers | homemade Windows game packs |

Review the list before confirming - that is exactly why the plugin does not delete anything silently.

## Feedback and contact

- Forum thread and downloads: https://forums.launchbox-app.com/files/file/5834-junk-cleaner-review-and-remove-non-game-entries-from-your-library/
- Bugs and false-positive reports (a file type that turned out to be a real game):
  https://github.com/tobiaswidjestam-design/launchbox-junk-cleaner/issues
- Built by **Aphex Studio Ryssby**, Sweden. Suggestions are welcome in the forum thread.

If a file type should not be in the list for your system, say which platform and extension and it
will be looked at - the extension list is meant to be a starting point, not a verdict.

## Uninstall

Delete the folder `LaunchBox\Plugins\JunkCleaner`. No other files are touched.

## Build from source

```
dotnet build -c Release
```

The project targets `net10.0-windows` and references the LaunchBox assemblies from
`C:\Users\<you>\LaunchBox\Core\Unbroken.LaunchBox.Plugins.dll` and `System.Drawing.Common.dll`
with `Private=false`, so they are not shipped with the plugin.

## Changelog

- **1.0.0** - review window with per-entry checkboxes, Select all/none, dry run, English UI, embedded icon.
- **0.1.0** - first working version: confirmation dialog listing up to 15 entries, hard-coded extension list.

## License

MIT - do what you want, no warranty. Removing library entries is your decision; the plugin never
deletes files from disk.
