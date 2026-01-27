using System;
using System.IO;
using System.Linq;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using ExifLibrary;

namespace ExifDateFixer
{
    /// <summary>
    /// Service for reading and writing EXIF metadata
    /// </summary>
    public class ExifService
    {
        /// <summary>
        /// Checks if a file has an EXIF date
        /// </summary>
        public bool HasExifDate(string filePath)
        {
            try
            {
                var directories = ImageMetadataReader.ReadMetadata(filePath);
                var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                
                if (subIfdDirectory != null)
                {
                    if (subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTime, out var dateTime1) && dateTime1 != DateTime.MinValue)
                        return true;

                    if (subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime2) && dateTime2 != DateTime.MinValue)
                        return true;

                    if (subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeDigitized, out var dateTime3) && dateTime3 != DateTime.MinValue)
                        return true;
                }

                return false;
            }
            catch
            {
                // If we can't read metadata, assume no EXIF date
                return false;
            }
        }

        /// <summary>
        /// Writes an EXIF date to a file
        /// </summary>
        public bool WriteExifDate(string filePath, DateTime dateTime)
        {
            try
            {
                var file = ImageFile.FromFile(filePath);
                
                // Set all three date fields
                file.Properties.Set(ExifTag.DateTime, dateTime);
                file.Properties.Set(ExifTag.DateTimeOriginal, dateTime);
                file.Properties.Set(ExifTag.DateTimeDigitized, dateTime);
                
                file.Save(filePath);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
