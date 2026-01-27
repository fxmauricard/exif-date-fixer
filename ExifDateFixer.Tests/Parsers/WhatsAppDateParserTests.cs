using System;
using ExifDateFixer.Parsers;
using Xunit;

namespace ExifDateFixer.Tests.Parsers
{
    public class WhatsAppDateParserTests
    {
        private readonly WhatsAppDateParser _parser;

        public WhatsAppDateParserTests()
        {
            _parser = new WhatsAppDateParser();
        }

        [Fact]
        public void Name_ReturnsWhatsApp()
        {
            // Arrange & Act
            var name = _parser.Name;

            // Assert
            Assert.Equal("WhatsApp", name);
        }

        [Theory]
        [InlineData("IMG-20230115-WA0001.jpg", 2023, 1, 15)]
        [InlineData("IMG-20210305-WA9999.jpeg", 2021, 3, 5)]
        [InlineData("VID-20220825-WA0042.jpg", 2022, 8, 25)]
        [InlineData("img-20201231-wa0000.jpg", 2020, 12, 31)]
        public void TryParse_ValidWhatsAppFormat_ReturnsTrue(string filename, int year, int month, int day)
        {
            // Act
            var result = _parser.TryParse(filename, out var dateTime);

            // Assert
            Assert.True(result);
            Assert.Equal(new DateTime(year, month, day), dateTime);
        }

        [Theory]
        [InlineData("20230115_123045.jpg")] // Samsung format
        [InlineData("WP_20230115_12_30_45_Pro.jpg")] // Windows Phone format
        [InlineData("photo.jpg")]
        [InlineData("IMG-20230115.jpg")] // Missing WA part
        [InlineData("IMG-2023011-WA0001.jpg")] // Invalid date (7 digits)
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
            var filename = "IMG-20231315-WA0001.jpg";

            // Act
            var result = _parser.TryParse(filename, out var dateTime);

            // Assert
            Assert.False(result);
        }
    }
}
