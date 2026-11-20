using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Unbroken.LaunchBox.Plugins;
using Unbroken.LaunchBox.Plugins.Data;

namespace JunkCleaner;

/// <summary>
/// Adds a Tools menu item that finds library entries whose ApplicationPath is not a game file
/// (dll, bat, ico, log, txt, images ...) and lets the user review and remove them.
/// Works through the LaunchBox plugin API - the XML/SQLite data files are never edited by hand.
/// </summary>
public sealed class JunkCleanerPlugin : ISystemMenuItemPlugin
{
    private const string CaptionText = "Junk Cleaner - remove non-game entries";

    private static readonly HashSet<string> JunkExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".dll", ".bat", ".cmd", ".ico", ".log", ".md5", ".txt", ".dat", ".json", ".ini",
        ".cfg", ".config", ".sys", ".so", ".dylib", ".pak", ".url", ".html", ".htm",
        ".nfo", ".sfv", ".xml", ".md", ".pdb", ".lib", ".tmp", ".bak", ".db", ".manifest",
        ".css", ".js", ".jpg", ".jpeg", ".png", ".webp", ".bmp", ".gif",
    };

    private static System.Drawing.Image? icon;
    private static bool iconLoaded;

    public string Caption => CaptionText;

    public System.Drawing.Image IconImage
    {
        get
        {
            if (!iconLoaded)
            {
                iconLoaded = true;
                icon = LoadIcon();
            }

            return icon!;
        }
    }

    public bool ShowInLaunchBox => true;

    public bool ShowInBigBox => false;

    public bool AllowInBigBoxWhenLocked => false;

    public void OnSelected()
    {
        try
        {
            var dataManager = PluginHelper.DataManager;
            if (dataManager is null)
            {
                Show("LaunchBox data is not available yet. Open LaunchBox and try again.", MessageBoxImage.Warning);
                return;
            }

            var games = dataManager.GetAllGames() ?? Array.Empty<IGame>();
            var junk = games
                .Where(IsJunk)
                .Select(game => (Game: game, File: FileName(game.ApplicationPath)))
                .ToList();

            if (junk.Count == 0)
            {
                Show($"No non-game entries found - the library is clean.{Environment.NewLine}{Environment.NewLine}{games.Length} entries checked.", MessageBoxImage.Information);
                return;
            }

            var window = new JunkCleanerWindow(junk);
            if (window.ShowDialog() != true)
            {
                return;
            }

            var ticked = window.Ticked;
            if (window.DryRun)
            {
                Show($"Dry run - nothing was changed.{Environment.NewLine}{Environment.NewLine}{ticked.Count} of {junk.Count} entries would be removed.", MessageBoxImage.Information);
                return;
            }

            if (ticked.Count == 0)
            {
                Show("Nothing was ticked, so nothing was removed.", MessageBoxImage.Information);
                return;
            }

            var removed = ticked.Count(dataManager.TryRemoveGame);
            dataManager.Save(true);

            Show($"Removed {removed} of {ticked.Count} ticked entries.{Environment.NewLine}"
                 + $"{(ticked.Count - removed)} could not be removed.", MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            Show("Junk Cleaner failed: " + ex.Message, MessageBoxImage.Error);
        }
    }

    private static bool IsJunk(IGame game)
    {
        var path = Clean(game.ApplicationPath);
        if (string.IsNullOrWhiteSpace(path))
        {
            return false;
        }

        var extension = Path.GetExtension(path);
        return !string.IsNullOrEmpty(extension) && JunkExtensions.Contains(extension);
    }

    private static string Clean(string? path) => path?.Trim().Trim('"') ?? string.Empty;

    private static string FileName(string? path)
    {
        var clean = Clean(path);
        if (clean.Length == 0)
        {
            return string.Empty;
        }

        try
        {
            return Path.GetFileName(clean);
        }
        catch (ArgumentException)
        {
            return clean;
        }
    }

    private static System.Drawing.Image? LoadIcon()
    {
        try
        {
            using var stream = typeof(JunkCleanerPlugin).Assembly.GetManifestResourceStream("JunkCleaner.icon32.png");
            if (stream is null)
            {
                return null;
            }

            using var buffer = new MemoryStream();
            stream.CopyTo(buffer);
            buffer.Position = 0;
            return System.Drawing.Image.FromStream(buffer);
        }
        catch
        {
            return null;
        }
    }

    private static void Show(string message, MessageBoxImage iconType)
        => MessageBox.Show(message, "Junk Cleaner", MessageBoxButton.OK, iconType);
}
