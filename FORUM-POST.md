# Forum post (LaunchBox → Downloads → Third-party Apps and Plugins)

**Title:**
`Junk Cleaner 1.0 - review and remove non-game entries from your library`

**Body:**

```
Junk Cleaner by Aphex Studio Ryssby, 2026

Automatic Import (and folder imports in general) pull in every file that sits next to
your ROMs - .dll, .bat, .ico, .log, .md5, .txt, .dat, .json, .nfo and cover images - and
each one becomes a "game" in the platform list. Deleting them by hand, one right-click at
a time, is the reason I wrote this.

WHAT IT DOES

Tools -> "Junk Cleaner - remove non-game entries" scans the library and lists every entry
whose ApplicationPath has a non-game extension, showing title, file name and platform.

  - every entry has a checkbox, so nothing is removed without you seeing it
  - Select all / Select none, and the button always shows the count: "Remove ticked (12)"
  - "Dry run (report only, change nothing)" lets you see what would go before touching anything
  - removal goes through the LaunchBox plugin API (IDataManager.TryRemoveGame + Save), so no
    XML editing, no restart, and no files are touched on disk - only library entries

REQUIREMENTS

  - LaunchBox for Windows 13.x or newer (built against the 14.1 plugin SDK, .NET 10)
  - Windows 10/11 x64

INSTALL

  1. Close LaunchBox
  2. Copy the JunkCleaner folder into LaunchBox\Plugins\
  3. Start LaunchBox -> Tools -> Junk Cleaner - remove non-game entries

PLEASE READ THIS FIRST

Some extensions in the list are legitimate games on specific systems: .bat/.cmd on DOS and
ScummVM setups, .png for Pico-8 carts, .json in some MUGEN/ScummVM/frontend sets. That is
exactly why the plugin shows a review list instead of deleting silently - untick anything
you want to keep, and use Dry run the first time on a large collection.

UNINSTALL

Delete LaunchBox\Plugins\JunkCleaner. Nothing else is changed.

SOURCE

Source and build instructions: https://github.com/tobiaswidjestam-design/launchbox-junk-cleaner

Feedback and false positives are welcome - if a file type in the list is a real game format
for a system I have missed, tell me and I will make the list configurable (or excluding it
per platform).

MIT licensed. Removing entries is your decision; the plugin never deletes files from disk.
```

**Upload file:** `JunkCleaner-1.0-AphexStudioRyssby.zip`

**Screenshot tip:** open the review window with a list that has a few entries and capture that -
the checkbox list with titles/file names/platforms is what convinces people to download.

## Reminders before posting

- Put the source on GitHub first (the whole `C:\HomeBrew\LaunchBoxJunkCleaner` folder minus
  `bin/`, `obj/`, `release/`, `versions/`) as the repo **launchbox-junk-cleaner**.
- Check the "Requirements" line against the LaunchBox version you actually tested on.
- Post the file under Downloads (not a new forum topic) - the forum rules ask for that.
