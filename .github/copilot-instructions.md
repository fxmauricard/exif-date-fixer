# Copilot Coding Agent Instructions - EXIF Date Fixer

## Repository Overview

**Purpose**: EXIF Date Fixer is a command-line tool that adds EXIF date metadata to JPEG/HEIF images based on filename patterns. It extracts dates from common filename formats (WhatsApp, Samsung, Windows Phone) and writes EXIF metadata (DateTimeOriginal, DateTimeDigitized, DateTime) to image files.

**Size & Type**: Small C# console application (~477 lines of code)
**Target Runtime**: .NET 10.0
**Language**: C# 
**Project Type**: Console Application (.NET SDK-style project)

## Project Structure

### Root Files
- `ExifDateFixer.sln` - Visual Studio solution file
- `ExifDateFixer.csproj` - Main project file (SDK-style)
- `Program.cs` - Entry point with command-line argument parsing (130 lines)
- `FileProcessor.cs` - File scanning and processing logic (143 lines)
- `ExifService.cs` - EXIF metadata reading/writing service (70 lines)
- `IFilenameDateParser.cs` - Interface for filename date parsers (23 lines)
- `WhatsAppDateParser.cs` - Parser for WhatsApp format: IMG-20230115-WA0001.jpg (35 lines)
- `SamsungDateParser.cs` - Parser for Samsung format: 20230115_123045.jpg (38 lines)
- `WindowsPhoneDateParser.cs` - Parser for Windows Phone format: WP_20230115_12_30_45_Pro.jpg (41 lines)
- `README.md` - Comprehensive documentation
- `.gitignore` - Standard Visual Studio .gitignore (331 lines)
- `LICENSE` - MIT License

### Configuration Files
- `.github/workflows/dotnet-desktop.yml` - GitHub Actions CI workflow
- No linting configuration files (no .editorconfig, stylecop.json, or .ruleset)

### Dependencies (from ExifDateFixer.csproj)
- **System.CommandLine** 2.0.0-beta4.22272.1 - Command-line argument parsing
- **MetadataExtractor** 2.9.0 - Reading EXIF metadata (note: README incorrectly states 2.8.1)
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

**CI Process** (runs on Windows):
1. Checkout code
2. Install .NET 10.0 SDK
3. Setup MSBuild.exe
4. Run: `msbuild ExifDateFixer.sln /t:Restore,Build /p:Configuration={Debug|Release}`

**Matrix Build**: Builds both Debug and Release configurations

**IMPORTANT**: The CI workflow uses MSBuild, NOT `dotnet build`. However, on Linux/macOS, use `dotnet build` as MSBuild is Windows-specific.

**To replicate CI locally on Linux/macOS**:
```bash
dotnet build -c Debug
dotnet build -c Release
```

## Code Architecture

### Application Flow
1. **Program.cs**: Entry point, defines CLI arguments, orchestrates processing
2. **FileProcessor**: Scans directories for supported files (.jpg, .jpeg, .heic, .heif), processes each file
3. **ExifService**: Checks for existing EXIF dates, writes new EXIF dates
4. **Parsers**: Try to extract dates from filenames using regex patterns

### Parser System (Extensible)
To add a new filename format parser:
1. Create a class implementing `IFilenameDateParser`
2. Implement `Name` property and `TryParse(string filename, out DateTime dateTime)` method
3. Register parser in `Program.cs` line 41-46 in the parsers list

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
2. **Maintain parser interface**: All parsers must implement `IFilenameDateParser`
3. **Keep command-line interface**: Don't remove `-r/--recursive` or path argument
4. **Preserve supported extensions**: Don't remove existing file extensions

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

## Common Development Tasks

### Add a new filename format parser
1. Create `MyFormatDateParser.cs` in repository root
2. Implement `IFilenameDateParser` interface
3. Add regex pattern to extract date/time from filename
4. Register in `Program.cs` parsers list (line 41-46)
5. Build and test with sample filenames

### Add a new file extension
1. Edit `Program.cs` line 38: Add extension to `supportedExtensions` array
2. Verify extension is supported by both MetadataExtractor and ExifLibNet

### Change output formatting
1. Edit `Program.cs` lines 63-127 (display and summary logic)
2. Avoid breaking the progress counter `[N/Total]` format

### Modify EXIF writing behavior
1. Edit `ExifService.cs` method `WriteExifDate` (lines 49-67)
2. See ExifLibrary documentation for available ExifTag values

## Important Notes for Agents

1. **Trust these instructions**: All build commands have been tested and validated. Don't search for build steps unless these fail.

2. **No MSBuild on Linux/macOS**: Use `dotnet build` instead. The CI workflow runs on Windows.

3. **No tests to run**: Don't try to run `dotnet test` expecting output. There are no unit tests.

4. **Clean builds when stuck**: If you see unexpected behavior, `dotnet clean && dotnet build` resolves most issues.

5. **Case-insensitive extensions**: File extension matching is case-insensitive (FileProcessor.cs handles this via Directory.GetFiles).

6. **Parser order matters**: Parsers are tried in order listed in Program.cs. First match wins.

7. **No database or state**: This is a stateless tool. Each run is independent.

8. **Error handling**: The application handles exceptions gracefully and continues processing other files.

9. **Package version note**: README lists MetadataExtractor 2.8.1, but .csproj uses 2.9.0 (the .csproj is authoritative).

10. **When examining code**: All source files are in the repository root (flat structure, no src/ directory).
