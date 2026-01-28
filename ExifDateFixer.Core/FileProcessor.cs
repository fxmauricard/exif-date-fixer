using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExifDateFixer.Parsers;
using ExifDateFixer.Services;

namespace ExifDateFixer
{
    /// <summary>
    /// Handles file processing and EXIF date fixing
    /// </summary>
    public class FileProcessor
    {
        private readonly ExifService _exifService;
        private readonly List<IFilenameDateParser> _parsers;
        private readonly string[] _supportedExtensions;

        /// <summary>
        /// Event raised when progress is made during processing
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressChanged;

        public FileProcessor(ExifService exifService, IEnumerable<IFilenameDateParser> parsers, IEnumerable<string> supportedExtensions)
        {
            _exifService = exifService;
            _parsers = parsers.ToList();
            _supportedExtensions = supportedExtensions.ToArray();
        }

        /// <summary>
        /// Raises the ProgressChanged event
        /// </summary>
        protected virtual void OnProgressChanged(ProgressEventArgs e)
        {
            ProgressChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Processes multiple files with progress reporting
        /// </summary>
        /// <param name="files">List of files to process</param>
        /// <param name="dryRun">If true, simulates processing without modifying files</param>
        public void ProcessFiles(List<string> files, bool dryRun = false)
        {
            for (int i = 0; i < files.Count; i++)
            {
                var file = files[i];
                var filename = Path.GetFileName(file);
                var result = ProcessFile(file, dryRun);

                OnProgressChanged(new ProgressEventArgs
                {
                    FilePath = file,
                    FileName = filename,
                    CurrentFile = i + 1,
                    TotalFiles = files.Count,
                    Result = result
                });
            }
        }

        /// <summary>
        /// Gets all supported image files in a directory
        /// </summary>
        public List<string> GetSupportedFiles(string path, bool recursive)
        {
            var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var files = new List<string>();

            try
            {
                foreach (var extension in _supportedExtensions)
                {
                    files.AddRange(System.IO.Directory.GetFiles(path, $"*{extension}", searchOption));
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Skip directories we don't have access to
            }
            catch (DirectoryNotFoundException)
            {
                // Path doesn't exist
            }

            return files;
        }

        /// <summary>
        /// Processes a single file
        /// </summary>
        /// <param name="filePath">Path to the file to process</param>
        /// <param name="dryRun">If true, simulates processing without modifying files</param>
        public ProcessingResult ProcessFile(string filePath, bool dryRun = false)
        {
            try
            {
                // Check if EXIF date already exists
                if (_exifService.HasExifDate(filePath))
                {
                    return new ProcessingResult
                    {
                        Status = ProcessingStatus.SkippedHasDate,
                        Message = "EXIF date already exists"
                    };
                }

                // Try to parse date from filename
                var filename = Path.GetFileName(filePath);
                DateTime parsedDate = default;
                IFilenameDateParser successfulParser = null;

                foreach (var parser in _parsers)
                {
                    if (parser.TryParse(filename, out parsedDate))
                    {
                        successfulParser = parser;
                        break;
                    }
                }

                if (successfulParser == null)
                {
                    return new ProcessingResult
                    {
                        Status = ProcessingStatus.SkippedNoDateInFilename,
                        Message = "Could not parse date from filename"
                    };
                }

                // In dry-run mode, don't actually write
                if (dryRun)
                {
                    return new ProcessingResult
                    {
                        Status = ProcessingStatus.WouldUpdate,
                        Message = $"Would add EXIF date {parsedDate:yyyy-MM-dd HH:mm:ss}",
                        ParsedDate = parsedDate,
                        ParserUsed = successfulParser.Name
                    };
                }

                // Write EXIF date
                if (_exifService.WriteExifDate(filePath, parsedDate))
                {
                    return new ProcessingResult
                    {
                        Status = ProcessingStatus.Updated,
                        Message = $"Added EXIF date {parsedDate:yyyy-MM-dd HH:mm:ss}",
                        ParsedDate = parsedDate,
                        ParserUsed = successfulParser.Name
                    };
                }
                else
                {
                    return new ProcessingResult
                    {
                        Status = ProcessingStatus.Error,
                        Message = "Failed to write EXIF data"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ProcessingResult
                {
                    Status = ProcessingStatus.Error,
                    Message = ex.Message
                };
            }
        }
    }

    /// <summary>
    /// Result of processing a file
    /// </summary>
    public class ProcessingResult
    {
        public ProcessingStatus Status { get; set; }
        public string Message { get; set; }
        public DateTime? ParsedDate { get; set; }
        public string ParserUsed { get; set; }
    }

    /// <summary>
    /// Status of file processing
    /// </summary>
    public enum ProcessingStatus
    {
        Updated,
        WouldUpdate,
        SkippedHasDate,
        SkippedNoDateInFilename,
        Error
    }

    /// <summary>
    /// Event args for progress reporting
    /// </summary>
    public class ProgressEventArgs : EventArgs
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public int CurrentFile { get; set; }
        public int TotalFiles { get; set; }
        public ProcessingResult Result { get; set; }
    }
}
