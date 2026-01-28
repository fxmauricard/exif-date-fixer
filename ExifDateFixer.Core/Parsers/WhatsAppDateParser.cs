using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ExifDateFixer.Parsers
{
    /// <summary>
    /// Parser for WhatsApp format filenames: IMG-20230115-WA0001.jpg -> 2023-01-15
    /// </summary>
    public class WhatsAppDateParser : IFilenameDateParser
    {
        private static readonly Regex Pattern = new Regex(
            @"(?:IMG|VID)-(\d{8})-WA\d+",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public string Name => "WhatsApp";

        public bool TryParse(string filename, out DateTime dateTime)
        {
            dateTime = default;
            var match = Pattern.Match(filename);
            
            if (!match.Success)
                return false;

            var dateString = match.Groups[1].Value;
            return DateTime.TryParseExact(
                dateString,
                "yyyyMMdd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateTime);
        }
    }
}
