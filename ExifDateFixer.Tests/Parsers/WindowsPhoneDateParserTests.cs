using System;
using ExifDateFixer.Parsers;
using Xunit;

namespace ExifDateFixer.Tests.Parsers
{
    public class WindowsPhoneDateParserTests
    {
        private readonly WindowsPhoneDateParser _parser;

        public WindowsPhoneDateParserTests()
        {
            _parser = new WindowsPhoneDateParser();
        }

        [Fact]
        public void Name_ReturnsWindowsPhone()
        {
            // Arrange & Act
            var name = _parser.Name;

            // Assert
            Assert.Equal("Windows Phone", name);
        }

        [Theory]
        [InlineData("WP_20230115_12_30_45_Pro.jpg", 2023, 1, 15, 12, 30, 45)]
        [InlineData("WP_20210305_09_00_00.jpeg", 2021, 3, 5, 9, 0, 0)]
        [InlineData("WP_20220825_23_59_59_Pro.jpg", 2022, 8, 25, 23, 59, 59)]
        [InlineData("WP_20201231_00_00_00.jpg", 2020, 12, 31, 0, 0, 0)]
        [InlineData("wp_20230115_12_30_45_pro.jpg", 2023, 1, 15, 12, 30, 45)] // Case insensitive
        public void TryParse_ValidWindowsPhoneFormat_ReturnsTrue(string filename, int year, int month, int day, int hour, int minute, int second)
        {
            // Act
            var result = _parser.TryParse(filename, out var dateTime);

            // Assert
            Assert.True(result);
            Assert.Equal(new DateTime(year, month, day, hour, minute, second), dateTime);
        }

        [Theory]
        [InlineData("IMG-20230115-WA0001.jpg")] // WhatsApp format
        [InlineData("20230115_123045.jpg")] // Samsung format
        [InlineData("photo.jpg")]
        [InlineData("WP_20230115.jpg")] // Missing time parts
        [InlineData("WP_2023011_12_30_45.jpg")] // Invalid date (7 digits)
        [InlineData("WP_20230115_2_30_45.jpg")] // Invalid hour (1 digit)
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
            var filename = "WP_20231315_12_30_45_Pro.jpg";

            // Act
            var result = _parser.TryParse(filename, out var dateTime);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void TryParse_InvalidTime_ReturnsFalse()
        {
            // Arrange - Invalid hour (25)
            var filename = "WP_20230115_25_30_45_Pro.jpg";

            // Act
            var result = _parser.TryParse(filename, out var dateTime);

            // Assert
            Assert.False(result);
        }
    }
}
