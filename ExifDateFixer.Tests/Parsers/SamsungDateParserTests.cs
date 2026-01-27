using System;
using ExifDateFixer.Parsers;
using Xunit;

namespace ExifDateFixer.Tests.Parsers
{
    public class SamsungDateParserTests
    {
        private readonly SamsungDateParser _parser;

        public SamsungDateParserTests()
        {
            _parser = new SamsungDateParser();
        }

        [Fact]
        public void Name_ReturnsSamsung()
        {
            // Arrange & Act
            var name = _parser.Name;

            // Assert
            Assert.Equal("Samsung", name);
        }

        [Theory]
        [InlineData("20230115_123045.jpg", 2023, 1, 15, 12, 30, 45)]
        [InlineData("20210305_090000.jpeg", 2021, 3, 5, 9, 0, 0)]
        [InlineData("20220825_235959.jpg", 2022, 8, 25, 23, 59, 59)]
        [InlineData("20201231_000000.jpg", 2020, 12, 31, 0, 0, 0)]
        public void TryParse_ValidSamsungFormat_ReturnsTrue(string filename, int year, int month, int day, int hour, int minute, int second)
        {
            // Act
            var result = _parser.TryParse(filename, out var dateTime);

            // Assert
            Assert.True(result);
            Assert.Equal(new DateTime(year, month, day, hour, minute, second), dateTime);
        }

        [Theory]
        [InlineData("IMG-20230115-WA0001.jpg")] // WhatsApp format
        [InlineData("WP_20230115_12_30_45_Pro.jpg")] // Windows Phone format
        [InlineData("photo.jpg")]
        [InlineData("20230115.jpg")] // Missing time part
        [InlineData("2023011_123045.jpg")] // Invalid date (7 digits)
        [InlineData("20230115_12304.jpg")] // Invalid time (5 digits)
        [InlineData("prefix_20230115_123045.jpg")] // Has prefix (pattern requires start of string)
        [InlineData("")]
        public void TryParse_InvalidFormat_ReturnsFalse(string filename)
        {
            // Act
            var result = _parser.TryParse(filename, out var dateTime);

            // Assert
            Assert.False(result);
            Assert.Equal(default(DateTime), dateTime);
        }

        [Fact]
        public void TryParse_InvalidDate_ReturnsFalse()
        {
            // Arrange - Invalid month (13)
            var filename = "20231315_123045.jpg";

            // Act
            var result = _parser.TryParse(filename, out var dateTime);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryParse_InvalidTime_ReturnsFalse()
        {
            // Arrange - Invalid hour (25)
            var filename = "20230115_253045.jpg";

            // Act
            var result = _parser.TryParse(filename, out var dateTime);

            // Assert
            Assert.False(result);
        }
    }
}
