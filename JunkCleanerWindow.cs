using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Unbroken.LaunchBox.Plugins.Data;

namespace JunkCleaner;

/// <summary>Review window: one checkbox per junk entry, plus dry-run switch.</summary>
internal sealed class JunkCleanerWindow : Window
{
    private static readonly Brush Text = new SolidColorBrush(Color.FromRgb(0xE6, 0xED, 0xF3));
    private static readonly Brush Muted = new SolidColorBrush(Color.FromRgb(0x8B, 0x94, 0x9E));
    private static readonly Brush Accent = new SolidColorBrush(Color.FromRgb(0x7E, 0xE7, 0x87));

    private readonly List<(IGame Game, CheckBox Box)> rows = new();
    private readonly Button removeButton;
    private readonly CheckBox dryRun;
    private readonly TextBlock summary;

    public JunkCleanerWindow(IReadOnlyList<(IGame Game, string File)> items)
    {
        Title = "Junk Cleaner";
        Width = 920;
        Height = 640;
        MinWidth = 640;
        MinHeight = 420;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;
        ShowInTaskbar = false;
        Background = new SolidColorBrush(Color.FromRgb(0x16, 0x1B, 0x22));

        var dock = new DockPanel { Margin = new Thickness(16) };

        var header = new StackPanel { Margin = new Thickness(0, 0, 0, 10) };
        header.Children.Add(new TextBlock
        {
            Text = "Non-game entries in your library",
            FontSize = 18,
            FontWeight = FontWeights.SemiBold,
            Foreground = Text,
        });
        header.Children.Add(new TextBlock
        {
            Text = "Ticked entries are removed from LaunchBox (files on disk are never touched). "
                 + "Check carefully: .bat/.exe can be real games on DOS, .png on Pico-8 and .json in some MUGEN/ScummVM sets.",
            TextWrapping = TextWrapping.Wrap,
            Foreground = Muted,
            Margin = new Thickness(0, 6, 0, 0),
        });
        summary = new TextBlock { Foreground = Accent, Margin = new Thickness(0, 8, 0, 0) };
        header.Children.Add(summary);
        header.Children.Add(new TextBlock
        {
            Text = "Aphex Studio Ryssby - 2026",
            Foreground = Muted,
            FontSize = 11,
            Margin = new Thickness(0, 4, 0, 0),
        });
        DockPanel.SetDock(header, Dock.Top);
        dock.Children.Add(header);

        var bar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 12, 0, 0) };

        var selectAll = Button("Select all");
        selectAll.Click += (_, _) => SetAll(true);
        var selectNone = Button("Select none");
        selectNone.Click += (_, _) => SetAll(false);

        dryRun = new CheckBox
        {
            Content = "Dry run (report only, change nothing)",
            Foreground = Text,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(18, 0, 18, 0),
        };

        removeButton = Button("Remove ticked");
        removeButton.Margin = new Thickness(0, 0, 8, 0);
        removeButton.Click += (_, _) => DialogResult = true;

        var cancel = Button("Cancel");
        cancel.Click += (_, _) => DialogResult = false;

        bar.Children.Add(selectAll);
        bar.Children.Add(selectNone);
        bar.Children.Add(dryRun);
        bar.Children.Add(removeButton);
        bar.Children.Add(cancel);
        DockPanel.SetDock(bar, Dock.Bottom);
        dock.Children.Add(bar);

        var panel = new StackPanel();
        foreach (var (game, file) in items)
        {
            var box = new CheckBox
            {
                IsChecked = true,
                Foreground = Text,
                Margin = new Thickness(4, 4, 4, 4),
                Content = BuildRow(game, file),
            };
            box.Checked += (_, _) => UpdateSummary();
            box.Unchecked += (_, _) => UpdateSummary();
            rows.Add((game, box));
            panel.Children.Add(box);
        }

        dock.Children.Add(new ScrollViewer
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
            Padding = new Thickness(4),
            Content = panel,
        });

        Content = dock;
        UpdateSummary();
    }

    public bool DryRun => dryRun.IsChecked == true;

    public IReadOnlyList<IGame> Ticked => rows.Where(r => r.Box.IsChecked == true).Select(r => r.Game).ToList();

    private static StackPanel BuildRow(IGame game, string file)
    {
        var row = new StackPanel { Orientation = Orientation.Horizontal };
        row.Children.Add(new TextBlock
        {
            Text = string.IsNullOrWhiteSpace(game.Title) ? "(no title)" : game.Title,
            Foreground = Text,
            MinWidth = 260,
            TextTrimming = TextTrimming.CharacterEllipsis,
        });
        row.Children.Add(new TextBlock
        {
            Text = file,
            Foreground = Muted,
            Margin = new Thickness(12, 0, 12, 0),
            MinWidth = 220,
            TextTrimming = TextTrimming.CharacterEllipsis,
        });
        row.Children.Add(new TextBlock
        {
            Text = game.Platform ?? string.Empty,
            Foreground = Muted,
            FontStyle = FontStyles.Italic,
        });
        return row;
    }

    private static Button Button(string caption) => new()
    {
        Content = caption,
        Padding = new Thickness(14, 6, 14, 6),
        Margin = new Thickness(0, 0, 8, 0),
        Background = new SolidColorBrush(Color.FromRgb(0x2D, 0x33, 0x3B)),
        Foreground = Text,
        BorderBrush = new SolidColorBrush(Color.FromRgb(0x3D, 0x44, 0x4D)),
    };

    private void SetAll(bool ticked)
    {
        foreach (var row in rows)
        {
            row.Box.IsChecked = ticked;
        }
    }

    private void UpdateSummary()
    {
        var ticked = rows.Count(r => r.Box.IsChecked == true);
        summary.Text = $"{ticked} of {rows.Count} entries ticked";
        removeButton.Content = $"Remove ticked ({ticked})";
        removeButton.IsEnabled = ticked > 0;
    }
}
