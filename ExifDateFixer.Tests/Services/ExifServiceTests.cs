using System;
using System.IO;
using ExifDateFixer.Services;
using Xunit;

namespace ExifDateFixer.Tests.Services
{
    public class ExifServiceTests : IDisposable
    {
        private readonly ExifService _service;
        private readonly string _testDirectory;

        public ExifServiceTests()
        {
            _service = new ExifService();
            _testDirectory = Path.Combine(Path.GetTempPath(), $"ExifDateFixerTests_{Guid.NewGuid()}");
            Directory.CreateDirectory(_testDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public void HasExifDate_NonExistentFile_ReturnsFalse()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.jpg");

            // Act
            var result = _service.HasExifDate(nonExistentFile);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void HasExifDate_InvalidFile_ReturnsFalse()
        {
            // Arrange - Create a text file with .jpg extension
            var invalidFile = Path.Combine(_testDirectory, "invalid.jpg");
            File.WriteAllText(invalidFile, "This is not a valid image file");

            // Act
            var result = _service.HasExifDate(invalidFile);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WriteExifDate_NonExistentFile_ReturnsFalse()
        {
            // Arrange
            var nonExistentFile = Path.Combine(_testDirectory, "nonexistent.jpg");
            var testDate = new DateTime(2023, 1, 15, 12, 30, 45);

            // Act
            var result = _service.WriteExifDate(nonExistentFile, testDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WriteExifDate_InvalidFile_ReturnsFalse()
        {
            // Arrange - Create a text file with .jpg extension
            var invalidFile = Path.Combine(_testDirectory, "invalid.jpg");
            File.WriteAllText(invalidFile, "This is not a valid image file");
            var testDate = new DateTime(2023, 1, 15, 12, 30, 45);

            // Act
            var result = _service.WriteExifDate(invalidFile, testDate);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void WriteExifDate_ValidJpegFile_ReturnsTrue()
        {
            // Arrange - Create a minimal valid JPEG file
            var testFile = Path.Combine(_testDirectory, "test.jpg");
            CreateMinimalJpeg(testFile);
            var testDate = new DateTime(2023, 1, 15, 12, 30, 45);

            // Act
            var writeResult = _service.WriteExifDate(testFile, testDate);

            // Assert
            Assert.True(writeResult);
        }

        [Fact]
        public void HasExifDate_AfterWritingDate_ReturnsTrue()
        {
            // Arrange - Create a minimal valid JPEG file
            var testFile = Path.Combine(_testDirectory, "test.jpg");
            CreateMinimalJpeg(testFile);
            var testDate = new DateTime(2023, 1, 15, 12, 30, 45);

            // Act
            var writeResult = _service.WriteExifDate(testFile, testDate);
            var hasDateResult = _service.HasExifDate(testFile);

            // Assert
            Assert.True(writeResult);
            Assert.True(hasDateResult);
        }

        [Fact]
        public void HasExifDate_FileWithoutExifData_ReturnsFalse()
        {
            // Arrange - Create a minimal valid JPEG file without EXIF data
            var testFile = Path.Combine(_testDirectory, "test.jpg");
            CreateMinimalJpeg(testFile);

            // Act
            var result = _service.HasExifDate(testFile);

            // Assert
            Assert.False(result);
        }

        /// <summary>
        /// Creates a minimal valid JPEG file for testing
        /// This creates a 1x1 pixel red JPEG image
        /// </summary>
        private void CreateMinimalJpeg(string filePath)
        {
            // Minimal JPEG file structure:
            // SOI (Start of Image) marker: FF D8
            // APP0 (JFIF header): FF E0 00 10 4A 46 49 46 00 01 01 00 00 01 00 01 00 00
            // SOF0 (Start of Frame): FF C0 00 11 08 00 01 00 01 03 01 22 00 02 11 01 03 11 01
            // DHT (Huffman Table): FF C4 00 1F 00 00 01 05 01 01 01 01 01 01 00 00 00 00 00 00 00 00 01 02 03 04 05 06 07 08 09 0A 0B
            // SOS (Start of Scan): FF DA 00 0C 03 01 00 02 11 03 11 00 3F 00
            // Image data: (minimal compressed data)
            // EOI (End of Image) marker: FF D9

            byte[] jpegData = new byte[]
            {
                // SOI
                0xFF, 0xD8,
                // APP0
                0xFF, 0xE0, 0x00, 0x10, 0x4A, 0x46, 0x49, 0x46, 0x00, 0x01, 0x01, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00,
                // SOF0 (1x1 image)
                0xFF, 0xC0, 0x00, 0x11, 0x08, 0x00, 0x01, 0x00, 0x01, 0x03, 0x01, 0x22, 0x00, 0x02, 0x11, 0x01, 0x03, 0x11, 0x01,
                // DHT
                0xFF, 0xC4, 0x00, 0x1F, 0x00, 0x00, 0x01, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B,
                // DHT
                0xFF, 0xC4, 0x00, 0xB5, 0x10, 0x00, 0x02, 0x01, 0x03, 0x03, 0x02, 0x04, 0x03, 0x05, 0x05, 0x04, 0x04, 0x00, 0x00, 0x01, 0x7D, 0x01, 0x02, 0x03, 0x00, 0x04, 0x11, 0x05, 0x12, 0x21, 0x31, 0x41, 0x06, 0x13, 0x51, 0x61, 0x07, 0x22, 0x71, 0x14, 0x32, 0x81, 0x91, 0xA1, 0x08, 0x23, 0x42, 0xB1, 0xC1, 0x15, 0x52, 0xD1, 0xF0, 0x24, 0x33, 0x62, 0x72, 0x82, 0x09, 0x0A, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x25, 0x26, 0x27, 0x28, 0x29, 0x2A, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39, 0x3A, 0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A, 0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x69, 0x6A, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78, 0x79, 0x7A, 0x83, 0x84, 0x85, 0x86, 0x87, 0x88, 0x89, 0x8A, 0x92, 0x93, 0x94, 0x95, 0x96, 0x97, 0x98, 0x99, 0x9A, 0xA2, 0xA3, 0xA4, 0xA5, 0xA6, 0xA7, 0xA8, 0xA9, 0xAA, 0xB2, 0xB3, 0xB4, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xC2, 0xC3, 0xC4, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0xD9, 0xDA, 0xE1, 0xE2, 0xE3, 0xE4, 0xE5, 0xE6, 0xE7, 0xE8, 0xE9, 0xEA, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5, 0xF6, 0xF7, 0xF8, 0xF9, 0xFA,
                // SOS
                0xFF, 0xDA, 0x00, 0x0C, 0x03, 0x01, 0x00, 0x02, 0x11, 0x03, 0x11, 0x00, 0x3F, 0x00,
                // Minimal compressed data for 1x1 red pixel
                0xFC, 0xFF, 0xC0,
                // EOI
                0xFF, 0xD9
            };

            File.WriteAllBytes(filePath, jpegData);
        }
    }
}
