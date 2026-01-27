# Copilot Coding Agent Instructions - EXIF Date Fixer

## Repository Overview

**Purpose**: EXIF Date Fixer is a command-line tool that adds EXIF date metadata to JPEG/HEIF images based on filename patterns. It extracts dates from common filename formats (WhatsApp, Samsung, Windows Phone) and writes EXIF metadata (DateTimeOriginal, DateTimeDigitized, DateTime) to image files.

**Size & Type**: Small C# console application (~500 lines of code)
**Target Runtime**: .NET 10.0
**Language**: C# 
**Project Type**: Console Application (.NET SDK-style project)
**License**: GNU General Public License v3.0 (GPL-3.0)
**Version**: 1.0.0

## Project Structure

### Root Files
- `ExifDateFixer.sln` - Visual Studio solution file
- `ExifDateFixer.csproj` - Main project file (SDK-style) with version information
- `README.md` - Comprehensive documentation
- `.gitignore` - Standard Visual Studio .gitignore (331 lines)
- `LICENSE` - GNU General Public License v3.0

### Source Code Structure (src/)
```
src/
├── Program.cs                          # Entry point with command-line argument parsing (140 lines)
├── FileProcessor.cs                    # File scanning and processing logic (145 lines)
├── Parsers/
│   ├── IFilenameDateParser.cs         # Interface for filename date parsers (23 lines)
│   ├── WhatsAppDateParser.cs          # WhatsApp format: IMG-20230115-WA0001.jpg (35 lines)
│   ├── SamsungDateParser.cs           # Samsung format: 20230115_123045.jpg (38 lines)
│   └── WindowsPhoneDateParser.cs      # Windows Phone format: WP_20230115_12_30_45_Pro.jpg (41 lines)
└── Services/
    └── ExifService.cs                 # EXIF metadata reading/writing service (70 lines)
```

### Namespaces
- `ExifDateFixer` - Main namespace (Program.cs, FileProcessor.cs, ProcessingResult, ProcessingStatus)
- `ExifDateFixer.Parsers` - All parser implementations (IFilenameDateParser and concrete parsers)
- `ExifDateFixer.Services` - Service layer (ExifService)

### Configuration Files
- `.github/workflows/dotnet-desktop.yml` - GitHub Actions CI workflow (cross-platform)
- `.github/copilot-instructions.md` - This file
- No linting configuration files (no .editorconfig, stylecop.json, or .ruleset)

### Dependencies (from ExifDateFixer.csproj)
- **System.CommandLine** 2.0.0-beta4.22272.1 - Command-line argument parsing
- **MetadataExtractor** 2.9.0 - Reading EXIF metadata
- **ExifLibNet** 2.1.4 - Writing EXIF metadata

### Build Output Directories (Git-ignored)
- `bin/Debug/net10.0/` - Debug build artifacts
- `bin/Release/net10.0/` - Release build artifacts
- `obj/` - Intermediate build files

## Build & Development Commands

### Prerequisites
- .NET 10.0 SDK (version 10.0.102 or compatible)
- **CRITICAL**: The project targets .NET 10.0, not .NET 8.0 or earlier versions

### Build Commands - TESTED AND VALIDATED

**ALWAYS run these commands from the repository root directory**.

1. **Restore Dependencies** (optional, runs automatically before build):
   ```bash
   dotnet restore
   ```
   - Takes ~3 seconds
   - Downloads NuGet packages if not cached
   - Not required before `dotnet build` or `dotnet run` (they restore automatically)

2. **Clean Build Artifacts**:
   ```bash
   dotnet clean
   ```
   - Takes <1 second
   - Removes bin/ and obj/ directories
   - Use this if you encounter build issues

3. **Build (Debug)**:
   ```bash
   dotnet build
   ```
   - Takes ~7 seconds on first build, ~1-2 seconds on incremental builds
   - Builds to `bin/Debug/net10.0/ExifDateFixer.dll`
   - Automatically runs restore if needed
   - **ALWAYS succeeds** with no warnings or errors on clean repository

4. **Build (Release)**:
   ```bash
   dotnet build -c Release
   ```
   - Takes ~1-2 seconds
   - Builds to `bin/Release/net10.0/ExifDateFixer.dll`
   - Use this for production builds

5. **Run the Application**:
   ```bash
   dotnet run -- [arguments]
   ```
   - Examples:
     - `dotnet run -- --help` - Show help
     - `dotnet run -- --version` - Show version
     - `dotnet run -- .` - Scan current directory
     - `dotnet run -- /path/to/images -r` - Scan recursively
   - Takes ~2 seconds to start

6. **Direct Execution** (after building):
   ```bash
   ./bin/Debug/net10.0/ExifDateFixer [arguments]
   # or
   dotnet bin/Debug/net10.0/ExifDateFixer.dll [arguments]
   ```

### Testing
**NO TESTS EXIST** - This project does not have a test suite. Running `dotnet test` will complete successfully but do nothing.

### Linting/Formatting
**NO LINTERS CONFIGURED** - No StyleCop, EditorConfig, or other code style tools are in use.

### Build Troubleshooting

**Common Issues & Solutions**:

1. **If build fails with "framework not found"**: Verify .NET 10.0 SDK is installed with `dotnet --version`

2. **If incremental build seems stale**: Run `dotnet clean` then `dotnet build`

3. **After making code changes**: Simply run `dotnet build` - no special steps needed

4. **Build output**: Successfully built DLL will be at `bin/Debug/net10.0/ExifDateFixer.dll` or `bin/Release/net10.0/ExifDateFixer.dll`

## CI/CD - GitHub Actions Workflow

**File**: `.github/workflows/dotnet-desktop.yml`

**Triggers**:
- Push to `master` branch
- Pull requests to `master` branch

**CI Process** (cross-platform matrix build):
1. Checkout code
2. Install .NET 10.0 SDK
3. Restore: `dotnet restore ExifDateFixer.sln`
4. Build: `dotnet build ExifDateFixer.sln --configuration {Debug|Release} --no-restore`

**Matrix Build**: 
- Configurations: Debug, Release
- Operating Systems: ubuntu-latest, windows-latest, macos-latest

**IMPORTANT**: The CI workflow uses `dotnet build`, which works on all platforms (Windows, Linux, macOS).

**To replicate CI locally**:
```bash
dotnet restore
dotnet build -c Debug
dotnet build -c Release
```

## Code Architecture

### Application Flow
1. **Program.cs** (namespace: `ExifDateFixer`): Entry point, defines CLI arguments, orchestrates processing
2. **FileProcessor** (namespace: `ExifDateFixer`): Scans directories for supported files (.jpg, .jpeg, .heic, .heif), processes each file
3. **ExifService** (namespace: `ExifDateFixer.Services`): Checks for existing EXIF dates, writes new EXIF dates
4. **Parsers** (namespace: `ExifDateFixer.Parsers`): Try to extract dates from filenames using regex patterns

### Parser System (Extensible)
To add a new filename format parser:
1. Create a class in `src/Parsers/` implementing `IFilenameDateParser` interface
2. Use namespace `ExifDateFixer.Parsers`
3. Implement `Name` property and `TryParse(string filename, out DateTime dateTime)` method
4. Register parser in `src/Program.cs` in the parsers list (around line 50-55)
5. Build and test with sample filenames

### Supported File Extensions (case-insensitive)
- `.jpg`
- `.jpeg`
- `.heic`
- `.heif`

### Processing Logic
For each file:
1. Check if EXIF date already exists (skip if yes)
2. Try each parser until one successfully extracts a date from filename
3. Write EXIF date to three fields: DateTime, DateTimeOriginal, DateTimeDigitized
4. Report result (Updated, Skipped, or Error)

## Making Code Changes

### Safe Change Guidelines

1. **Preserve existing behavior**: Files with EXIF dates are always skipped
2. **Maintain parser interface**: All parsers must implement `IFilenameDateParser` in `ExifDateFixer.Parsers` namespace
3. **Keep command-line interface**: Don't remove `-r/--recursive`, `--version`, or path argument
4. **Preserve supported extensions**: Don't remove existing file extensions
5. **Maintain namespace organization**: Keep files in appropriate directories and use correct namespaces

### After Making Changes

1. **Build** to check for compilation errors:
   ```bash
   dotnet build
   ```

2. **Test manually** if changing parsing logic or file processing:
   ```bash
   # Create test directory with sample files
   mkdir -p /tmp/test-images
   # Add test files, then run:
   dotnet run -- /tmp/test-images
   ```

3. **Verify no regressions**: The tool should still:
   - Skip files with existing EXIF dates
   - Handle files it can't parse gracefully
   - Process all supported extensions
   - Support recursive scanning
   - Show version with `--version`

## Key Implementation Details

### EXIF Reading vs Writing Libraries
- **MetadataExtractor**: Used for READING existing EXIF data (fast, read-only)
- **ExifLibNet**: Used for WRITING EXIF data (can modify files)
- Both are necessary - don't remove either dependency

### File Processing Results
- **Updated**: Date parsed from filename and written to EXIF
- **SkippedHasDate**: EXIF date already exists (most common for processed files)
- **SkippedNoDateInFilename**: No parser could extract a date
- **Error**: Exception occurred (file access, corrupted file, etc.)

### Command-Line Arguments (System.CommandLine)
- Uses beta version 2.0.0-beta4.22272.1
- `pathArgument`: Directory to scan (default: current directory)
- `recursiveOption`: `-r` or `--recursive` flag
- `--version`: Displays version (built-in to System.CommandLine)

### Version Management
- Version is defined in `ExifDateFixer.csproj`:
  - `<Version>1.0.0</Version>`
  - `<AssemblyVersion>1.0.0.0</AssemblyVersion>`
  - `<FileVersion>1.0.0.0</FileVersion>`
- Version is displayed via `--version` command line option
- Version is shown in application header during execution

## Common Development Tasks

### Add a new filename format parser
1. Create `MyFormatDateParser.cs` in `src/Parsers/` directory
2. Use namespace `ExifDateFixer.Parsers`
3. Implement `IFilenameDateParser` interface
4. Add regex pattern to extract date/time from filename
5. Register in `src/Program.cs` parsers list
6. Build and test with sample filenames

### Add a new file extension
1. Edit `src/Program.cs`: Add extension to `supportedExtensions` array
2. Verify extension is supported by both MetadataExtractor and ExifLibNet

### Change output formatting
1. Edit `src/Program.cs` (display and summary logic in ProcessFiles method)
2. Avoid breaking the progress counter `[N/Total]` format

### Modify EXIF writing behavior
1. Edit `src/Services/ExifService.cs` method `WriteExifDate`
2. See ExifLibrary documentation for available ExifTag values

### Update version number
1. Edit `ExifDateFixer.csproj` properties: `Version`, `AssemblyVersion`, `FileVersion`
2. Rebuild the project

## Important Notes for Agents

1. **Trust these instructions**: All build commands have been tested and validated. Don't search for build steps unless these fail.

2. **Cross-platform support**: Use `dotnet build` for all platforms. MSBuild is not used anymore.

3. **No tests to run**: Don't try to run `dotnet test` expecting output. There are no unit tests.

4. **Clean builds when stuck**: If you see unexpected behavior, `dotnet clean && dotnet build` resolves most issues.

5. **Case-insensitive extensions**: File extension matching is case-insensitive (FileProcessor.cs handles this via Directory.GetFiles).

6. **Parser order matters**: Parsers are tried in order listed in Program.cs. First match wins.

7. **No database or state**: This is a stateless tool. Each run is independent.

8. **Error handling**: The application handles exceptions gracefully and continues processing other files.

9. **Namespace organization**: 
   - All source files are in `src/` directory and subdirectories
   - Use `ExifDateFixer.Parsers` for parsers
   - Use `ExifDateFixer.Services` for services
   - Use `ExifDateFixer` for main application logic

10. **License**: This project is licensed under GPL-3.0, not MIT. Always respect GPL-3.0 license terms when making changes.
