using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ExifDateFixer.Parsers;
using ExifDateFixer.Services;

namespace ExifDateFixer.Avalonia;

public partial class MainWindow : Window
{
    private bool _isProcessing = false;

    public MainWindow()
    {
        InitializeComponent();
        Title = "EXIF Date Fixer";
        
        // Set version from assembly
        var version = Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion ?? "1.0.0";
        VersionTextBlock.Text = $"Version {version}";
    }

    /// <summary>
    /// Browse button click handler - opens folder picker
    /// </summary>
    private async void BrowseButton_Click(object? sender, RoutedEventArgs e)
    {
        var folder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Folder to Scan",
            AllowMultiple = false
        });

        if (folder.Count > 0)
        {
            PathTextBox.Text = folder[0].Path.LocalPath;
        }
    }

    /// <summary>
    /// Start button click handler - begins processing
    /// </summary>
    private async void StartButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_isProcessing)
        {
            return;
        }

        var path = PathTextBox.Text;
        if (string.IsNullOrWhiteSpace(path))
        {
            await ShowErrorDialog("Please select a folder to process.");
            return;
        }

        if (!Directory.Exists(path))
        {
            await ShowErrorDialog($"The path '{path}' does not exist.");
            return;
        }

        _isProcessing = true;
        UpdateUIState(true);

        try
        {
            await ProcessFilesAsync(path, RecursiveCheckBox.IsChecked == true, DryRunCheckBox.IsChecked == true);
        }
        catch (Exception ex)
        {
            await ShowErrorDialog($"An error occurred: {ex.Message}");
        }
        finally
        {
            _isProcessing = false;
            UpdateUIState(false);
        }
    }

    /// <summary>
    /// Clear log button click handler
    /// </summary>
    private void ClearLogButton_Click(object? sender, RoutedEventArgs e)
    {
        LogTextBlock.Text = string.Empty;
        StatusTextBlock.Text = "Ready";
    }

    /// <summary>
    /// Process files asynchronously
    /// </summary>
    private async Task ProcessFilesAsync(string path, bool recursive, bool dryRun)
    {
        await Task.Run(() =>
        {
            // Initialize parsers
            var parsers = new List<IFilenameDateParser>
            {
                new WhatsAppDateParser(),
                new SamsungDateParser(),
                new WindowsPhoneDateParser()
            };

            // Supported file extensions (case-insensitive)
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".heic", ".heif" };

            // Initialize services
            var exifService = new ExifService();
            var processor = new FileProcessor(exifService, parsers, supportedExtensions);

            // Subscribe to progress events
            processor.ProgressChanged += (s, args) =>
            {
                Dispatcher.UIThread.Post(() =>
                {
                    // Update progress
                    if (ProgressBar != null)
                    {
                        ProgressBar.Value = (args.CurrentFile * 100.0) / args.TotalFiles;
                    }
                    
                    if (StatusTextBlock != null)
                    {
                        StatusTextBlock.Text = $"Processing {args.CurrentFile}/{args.TotalFiles}: {args.FileName}";
                    }

                    // Add log entry
                    var status = GetStatusIcon(args.Result.Status);
                    var message = $"[{args.CurrentFile}/{args.TotalFiles}] {args.FileName}\n  {status} {args.Result.Message}\n\n";
                    
                    if (LogTextBlock != null)
                    {
                        LogTextBlock.Text += message;
                    }

                    // Auto-scroll to bottom
                    if (LogScrollViewer != null)
                    {
                        LogScrollViewer.ScrollToEnd();
                    }
                });
            };

            // Log header
            Dispatcher.UIThread.Post(() =>
            {
                var dryRunText = dryRun ? " (DRY RUN)" : "";
                if (LogTextBlock != null)
                {
                    LogTextBlock.Text = $"EXIF Date Fixer - Scanning: {path}{dryRunText}\n";
                    LogTextBlock.Text += $"Recursive: {(recursive ? "Yes" : "No")}\n";
                    LogTextBlock.Text += $"Dry Run: {(dryRun ? "Yes" : "No")}\n";
                    LogTextBlock.Text += $"Supported extensions: {string.Join(", ", supportedExtensions)}\n\n";
                    LogTextBlock.Text += "Finding files...\n";
                }
            });

            // Get files
            var files = processor.GetSupportedFiles(path, recursive);

            if (files.Count == 0)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    if (LogTextBlock != null)
                    {
                        LogTextBlock.Text += "No supported files found.\n";
                    }
                    if (StatusTextBlock != null)
                    {
                        StatusTextBlock.Text = "No files found";
                    }
                });
                return;
            }

            Dispatcher.UIThread.Post(() =>
            {
                if (LogTextBlock != null)
                {
                    LogTextBlock.Text += $"Found {files.Count} files to process...\n\n";
                }
                if (ProgressBar != null)
                {
                    ProgressBar.Maximum = 100;
                    ProgressBar.Value = 0;
                }
            });

            // Process files
            processor.ProcessFiles(files, dryRun);

            // Display summary
            Dispatcher.UIThread.Post(() =>
            {
                var updated = 0;
                var wouldUpdate = 0;
                var skippedHasDate = 0;
                var skippedNoDate = 0;
                var errors = 0;

                foreach (var file in files)
                {
                    var result = processor.ProcessFile(file, dryRun);
                    switch (result.Status)
                    {
                        case ProcessingStatus.Updated:
                            updated++;
                            break;
                        case ProcessingStatus.WouldUpdate:
                            wouldUpdate++;
                            break;
                        case ProcessingStatus.SkippedHasDate:
                            skippedHasDate++;
                            break;
                        case ProcessingStatus.SkippedNoDateInFilename:
                            skippedNoDate++;
                            break;
                        case ProcessingStatus.Error:
                            errors++;
                            break;
                    }
                }

                if (LogTextBlock != null)
                {
                    LogTextBlock.Text += "Summary:\n";
                    LogTextBlock.Text += $"- Total files: {files.Count}\n";
                    if (dryRun)
                    {
                        LogTextBlock.Text += $"- Would update: {wouldUpdate}\n";
                    }
                    else
                    {
                        LogTextBlock.Text += $"- Updated: {updated}\n";
                    }
                    LogTextBlock.Text += $"- Skipped (has date): {skippedHasDate}\n";
                    LogTextBlock.Text += $"- Skipped (no date in filename): {skippedNoDate}\n";
                    LogTextBlock.Text += $"- Errors: {errors}\n";
                }

                if (StatusTextBlock != null)
                {
                    StatusTextBlock.Text = $"Completed: {files.Count} files processed";
                }
                
                if (ProgressBar != null)
                {
                    ProgressBar.Value = 100;
                }
            });
        });
    }

    /// <summary>
    /// Get status icon for result
    /// </summary>
    private string GetStatusIcon(ProcessingStatus status)
    {
        return status switch
        {
            ProcessingStatus.Updated => "✓",
            ProcessingStatus.WouldUpdate => "→",
            ProcessingStatus.SkippedHasDate => "-",
            ProcessingStatus.SkippedNoDateInFilename => "-",
            ProcessingStatus.Error => "✗",
            _ => "?"
        };
    }

    /// <summary>
    /// Update UI state based on processing status
    /// </summary>
    private void UpdateUIState(bool isProcessing)
    {
        StartButton.IsEnabled = !isProcessing;
        BrowseButton.IsEnabled = !isProcessing;
        PathTextBox.IsEnabled = !isProcessing;
        RecursiveCheckBox.IsEnabled = !isProcessing;
        DryRunCheckBox.IsEnabled = !isProcessing;
        ProgressBar.IsVisible = isProcessing;

        if (!isProcessing)
        {
            ProgressBar.Value = 0;
        }
    }

    /// <summary>
    /// Show error dialog
    /// </summary>
    private async Task ShowErrorDialog(string message)
    {
        var dialog = new Window
        {
            Title = "Error",
            Width = 400,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new StackPanel
            {
                Margin = new global::Avalonia.Thickness(20),
                Spacing = 15,
                Children =
                {
                    new TextBlock { Text = message, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
                    new Button 
                    { 
                        Content = "OK", 
                        HorizontalAlignment = global::Avalonia.Layout.HorizontalAlignment.Center,
                        MinWidth = 100
                    }
                }
            }
        };

        var button = ((StackPanel)dialog.Content!).Children[1] as Button;
        if (button != null)
        {
            button.Click += (s, e) => dialog.Close();
        }

        await dialog.ShowDialog(this);
    }
}