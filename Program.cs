using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ExifDateFixer.Parsers;
using ExifDateFixer.Services;

namespace ExifDateFixer
{
    class Program
    {
        private static string GetVersion()
        {
            var version = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion ?? "1.0.0";
            return version;
        }

        static async Task<int> Main(string[] args)
        {
            // Define command-line options
            var pathArgument = new Argument<string>(
                name: "path",
                description: "The path to scan for image files",
                getDefaultValue: () => ".");

            var recursiveOption = new Option<bool>(
                aliases: new[] { "-r", "--recursive" },
                description: "Scan directories recursively");

            var dryRunOption = new Option<bool>(
                aliases: new[] { "-d", "--dry-run" },
                description: "Preview changes without modifying files");

            var rootCommand = new RootCommand("EXIF Date Fixer - Adds EXIF date metadata to images based on filename patterns")
            {
                pathArgument,
                recursiveOption,
                dryRunOption
            };

            rootCommand.SetHandler(ProcessFiles, pathArgument, recursiveOption, dryRunOption);

            return await rootCommand.InvokeAsync(args);
        }

        static void ProcessFiles(string path, bool recursive, bool dryRun)
        {
            // Supported file extensions (case-insensitive)
            var supportedExtensions = new[] { ".jpg", ".jpeg", ".heic", ".heif" };

            // Initialize parsers
            var parsers = new List<IFilenameDateParser>
            {
                new WhatsAppDateParser(),
                new SamsungDateParser(),
                new WindowsPhoneDateParser()
            };

            // Initialize services
            var exifService = new ExifService();
            var processor = new FileProcessor(exifService, parsers, supportedExtensions);

            // Validate path
            if (!Directory.Exists(path))
            {
                Console.WriteLine($"Error: Path '{path}' does not exist.");
                return;
            }

            // Get absolute path for display
            var absolutePath = Path.GetFullPath(path);

            // Display header
            var dryRunText = dryRun ? " (DRY RUN)" : "";
            Console.WriteLine($"EXIF Date Fixer v{GetVersion()} - Scanning: " + absolutePath + dryRunText);
            Console.WriteLine($"Recursive: {(recursive ? "Yes" : "No")}");
            Console.WriteLine($"Dry Run: {(dryRun ? "Yes" : "No")}");
            Console.WriteLine($"Supported extensions: {string.Join(", ", supportedExtensions)}");
            Console.WriteLine();

            // Find all supported files
            Console.WriteLine("Finding files...");
            var files = processor.GetSupportedFiles(path, recursive);
            
            if (files.Count == 0)
            {
                Console.WriteLine("No supported files found.");
                return;
            }

            Console.WriteLine($"Found {files.Count} files to process...");
            Console.WriteLine();

            // Statistics
            int updated = 0;
            int wouldUpdate = 0;
            int skippedHasDate = 0;
            int skippedNoDate = 0;
            int errors = 0;

            // Process each file
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var filename = Path.GetFileName(file);

                Console.WriteLine($"[{i + 1}/{files.Count}] {filename}");

                var result = processor.ProcessFile(file, dryRun);

                switch (result.Status)
                {
                    case ProcessingStatus.Updated:
                        Console.WriteLine($"  ✓ Updated: {result.Message}");
                        updated++;
                        break;
                    case ProcessingStatus.WouldUpdate:
                        Console.WriteLine($"  → Would update: {result.Message}");
                        wouldUpdate++;
                        break;
                    case ProcessingStatus.SkippedHasDate:
                        Console.WriteLine($"  - Skipped: {result.Message}");
                        skippedHasDate++;
                        break;
                    case ProcessingStatus.SkippedNoDateInFilename:
                        Console.WriteLine($"  - Skipped: {result.Message}");
                        skippedNoDate++;
                        break;
                    case ProcessingStatus.Error:
                        Console.WriteLine($"  ✗ Error: {result.Message}");
                        errors++;
                        break;
                }

                Console.WriteLine();
            }

            // Display summary
            Console.WriteLine("Summary:");
            Console.WriteLine($"- Total files: {files.Count}");
            if (dryRun)
            {
                Console.WriteLine($"- Would update: {wouldUpdate}");
            }
            else
            {
                Console.WriteLine($"- Updated: {updated}");
            }
            Console.WriteLine($"- Skipped (has date): {skippedHasDate}");
            Console.WriteLine($"- Skipped (no date in filename): {skippedNoDate}");
            Console.WriteLine($"- Errors: {errors}");
        }
    }
}
