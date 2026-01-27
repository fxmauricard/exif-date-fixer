using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ExifDateFixer
{
    /// <summary>
    /// Parser for Windows Phone format filenames: WP_20230115_12_30_45_Pro.jpg -> 2023-01-15 12:30:45
    /// </summary>
    public class WindowsPhoneDateParser : IFilenameDateParser
    {
        private static readonly Regex Pattern = new Regex(
            @"WP_(\d{8})_(\d{2})_(\d{2})_(\d{2})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string Name => "Windows Phone";

        public bool TryParse(string filename, out DateTime dateTime)
        {
            dateTime = default;
            var match = Pattern.Match(filename);
            
            if (!match.Success)
                return false;

            var dateString = match.Groups[1].Value;
            var hour = match.Groups[2].Value;
            var minute = match.Groups[3].Value;
            var second = match.Groups[4].Value;
            var combinedString = $"{dateString}{hour}{minute}{second}";

            return DateTime.TryParseExact(
                combinedString,
                "yyyyMMddHHmmss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateTime);
        }
    }
}
