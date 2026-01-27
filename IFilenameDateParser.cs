using System;

namespace ExifDateFixer
{
    /// <summary>
    /// Interface for parsing dates from filenames
    /// </summary>
    public interface IFilenameDateParser
    {
        /// <summary>
        /// Gets the name of this parser for display purposes
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Attempts to parse a date from a filename
        /// </summary>
        /// <param name="filename">The filename to parse</param>
        /// <param name="dateTime">The parsed DateTime if successful</param>
        /// <returns>True if the date was successfully parsed, false otherwise</returns>
        bool TryParse(string filename, out DateTime dateTime);
    }
}
