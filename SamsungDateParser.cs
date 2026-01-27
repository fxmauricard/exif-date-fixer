using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ExifDateFixer
{
    /// <summary>
    /// Parser for Samsung format filenames: 20230115_123045.jpg -> 2023-01-15 12:30:45
    /// </summary>
    public class SamsungDateParser : IFilenameDateParser
    {
        private static readonly Regex Pattern = new Regex(
            @"^(\d{8})_(\d{6})",
            RegexOptions.Compiled);

        public string Name => "Samsung";

        public bool TryParse(string filename, out DateTime dateTime)
        {
            dateTime = default;
            var match = Pattern.Match(filename);
            
            if (!match.Success)
                return false;

            var dateString = match.Groups[1].Value;
            var timeString = match.Groups[2].Value;
            var combinedString = dateString + timeString;

            return DateTime.TryParseExact(
                combinedString,
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateTime);
        }
    }
}
