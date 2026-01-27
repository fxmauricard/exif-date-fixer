# EXIF Date Fixer

A command-line tool to add EXIF date metadata to JPEG/HEIF images based on filename patterns.

## Features

- **Automatic Date Parsing**: Extracts dates from common filename formats
- **EXIF Metadata Writing**: Adds DateTimeOriginal, DateTimeDigitized, and DateTime EXIF tags
- **Extensible Parser System**: Easy to add new filename format parsers
- **Recursive Scanning**: Optional recursive directory traversal
- **Progress Reporting**: Clear console output with detailed statistics

## Supported Filename Formats

The tool currently supports three common filename patterns:

1. **WhatsApp Format**: `IMG-20230115-WA0001.jpg` → `2023-01-15`
2. **Samsung Format**: `20230115_123045.jpg` → `2023-01-15 12:30:45`
3. **Windows Phone Format**: `WP_20230115_12_30_45_Pro.jpg` → `2023-01-15 12:30:45`

## Supported File Extensions

- `.jpg`
- `.jpeg`
- `.heic`
- `.heif`

(Case-insensitive)

## Installation

### Prerequisites

- .NET 10.0 SDK or later

### Build from Source

```bash
git clone https://github.com/fxmauricard/exif-date-fixer.git
cd exif-date-fixer
dotnet build -c Release
```

The compiled executable will be in `bin/Release/net10.0/`

## Usage

### Basic Usage

```bash
# Scan current directory
ExifDateFixer .

# Scan specific path
ExifDateFixer "C:\Photos"

# Scan specific path on Linux/Mac
ExifDateFixer "/home/user/photos"
```

### Recursive Scanning

```bash
# Scan directory and all subdirectories
ExifDateFixer "C:\Photos" -r
ExifDateFixer "C:\Photos" --recursive
```

### Help

```bash
# Show help information
ExifDateFixer --help
ExifDateFixer -h
```

## Example Output

```
EXIF Date Fixer - Scanning: C:\Photos
Recursive: Yes
Supported extensions: .jpg, .jpeg, .heic, .heif

Found 150 files to process...

[1/150] IMG-20230115-WA0001.jpg
  ✓ Updated: Added EXIF date 2023-01-15 00:00:00

[2/150] photo.jpg
  - Skipped: Could not parse date from filename

[3/150] 20230120_143022.jpg
  ✓ Updated: Added EXIF date 2023-01-20 14:30:22

[4/150] vacation.jpg
  - Skipped: EXIF date already exists

...

Summary:
- Total files: 150
- Updated: 45
- Skipped (has date): 80
- Skipped (no date in filename): 20
- Errors: 5
```

## How It Works

For each image file:

1. **Check Existing EXIF**: Verifies if EXIF date metadata already exists
2. **Parse Filename**: Attempts to extract a date using registered parsers
3. **Write EXIF Data**: If a date is found and no EXIF date exists, writes the date to EXIF metadata
4. **Report Progress**: Displays the action taken for each file

## Adding New Parsers

To add support for a new filename format:

1. Create a new class implementing `IFilenameDateParser`
2. Implement the `TryParse` method with your pattern matching logic
3. Register the parser in `Program.cs` by adding it to the parsers list

Example:

```csharp
public class MyCustomDateParser : IFilenameDateParser
{
    public string Name => "MyCustom";

    public bool TryParse(string filename, out DateTime dateTime)
    {
        dateTime = default;
        // Your parsing logic here
        return false;
    }
}
```

## Dependencies

- **System.CommandLine** (2.0.0-beta4.22272.1): Command-line argument parsing
- **MetadataExtractor** (2.9.0): Reading EXIF metadata
- **ExifLibNet** (2.1.4): Writing EXIF metadata

## Development

### Building the Project

```bash
# Build the solution
dotnet build

# Build in Release mode
dotnet build -c Release
```

### Running Tests

The project includes comprehensive unit tests for all parsers and the ExifService.

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run tests for a specific project
dotnet test ExifDateFixer.Tests/ExifDateFixer.Tests.csproj
```

#### Test Coverage

The test suite includes:

- **Parser Tests**: Validate all filename date parsers (WhatsApp, Samsung, Windows Phone)
  - Valid format parsing
  - Invalid format handling
  - Edge cases (invalid dates, malformed filenames)
  - Case sensitivity
  
- **ExifService Tests**: Validate EXIF metadata operations
  - Reading EXIF dates from images
  - Writing EXIF dates to images
  - Error handling for invalid/missing files

All tests use xUnit framework and are located in the `ExifDateFixer.Tests` project.

## Technical Details

- Writes to three EXIF date fields: `DateTime`, `DateTimeOriginal`, and `DateTimeDigitized`
- Preserves existing EXIF metadata when writing dates
- Handles file access errors gracefully
- Skips files that already have EXIF dates to prevent overwriting

## License

This project is licensed under the GNU General Public License v3.0 - see the LICENSE file for details.

