# Copilot Coding Agent Instructions - EXIF Date Fixer

## Repository Overview

**Purpose**: EXIF Date Fixer is a tool that adds EXIF date metadata to JPEG/HEIF images based on filename patterns. It extracts dates from common filename formats (WhatsApp, Samsung, Windows Phone) and writes EXIF metadata (DateTimeOriginal, DateTimeDigitized, DateTime) to image files.

**Available Interfaces**: 
- Command-line application (Windows/Linux/macOS)
- WinUI 3 GUI application (Windows only)

**Size & Type**: Multi-project C# solution with shared business logic
**Target Runtime**: .NET 10.0
**Language**: C# 
**License**: GNU General Public License v3.0 (GPL-3.0)
**Version**: 1.0.0

## Project Structure

### Solution Organization

The solution consists of four projects:

1. **ExifDateFixer.Core** - Shared business logic library (platform-agnostic)
2. **ExifDateFixer** - Command-line interface (cross-platform)
3. **ExifDateFixer.WinUI** - Windows GUI application (Windows-only)
4. **ExifDateFixer.Tests** - Unit tests (xUnit)

### Root Files
- `ExifDateFixer.sln` - Visual Studio solution file (includes all 4 projects)
- `README.md` - Comprehensive documentation
- `BUILDING.md` - Cross-platform build instructions
- `IMPLEMENTATION_SUMMARY.md` - Architecture and implementation details
- `.gitignore` - Standard Visual Studio .gitignore
- `LICENSE` - GNU General Public License v3.0

### ExifDateFixer.Core (Business Logic Library)
```
ExifDateFixer.Core/
├── FileProcessor.cs                    # File scanning and processing with progress events
├── Parsers/
│   ├── IFilenameDateParser.cs         # Interface for filename date parsers
│   ├── WhatsAppDateParser.cs          # WhatsApp format: IMG-20230115-WA0001.jpg
│   ├── SamsungDateParser.cs           # Samsung format: 20230115_123045.jpg
│   └── WindowsPhoneDateParser.cs      # Windows Phone format: WP_20230115_12_30_45_Pro.jpg
└── Services/
    └── ExifService.cs                 # EXIF metadata reading/writing service
```

**Dependencies**:
- MetadataExtractor 2.9.0 - Reading EXIF metadata
- ExifLibNet 2.1.4 - Writing EXIF metadata

**Key Features**:
- Dry-run mode support (preview without modifying files)
- Progress reporting via events (ProgressChanged)
- Platform-agnostic implementation

### ExifDateFixer (CLI Application)
```
ExifDateFixer/
├── Program.cs                          # Entry point with command-line argument parsing
└── ExifDateFixer.csproj                # References ExifDateFixer.Core
```

**Dependencies**:
- System.CommandLine 2.0.0-beta4.22272.1 - Command-line argument parsing
- ExifDateFixer.Core (project reference)

**Command-Line Options**:
- `path` - Directory to scan (default: current directory)
- `-r, --recursive` - Scan subdirectories recursively
- `-d, --dry-run` - Preview changes without modifying files
- `--version` - Show version information
- `-h, --help` - Show help information

### ExifDateFixer.WinUI (Windows GUI)
```
ExifDateFixer.WinUI/
├── App.xaml                            # Application entry point
├── App.xaml.cs
├── MainWindow.xaml                     # Main window UI
├── MainWindow.xaml.cs                  # Main window logic
├── app.manifest                        # Windows compatibility manifest
└── ExifDateFixer.WinUI.csproj          # References ExifDateFixer.Core
```

**Dependencies**:
- Microsoft.WindowsAppSDK 1.6.250116000 - WinUI 3 framework
- Microsoft.Windows.SDK.BuildTools 10.0.26100.1742 - Windows SDK
- ExifDateFixer.Core (project reference)

**Requirements**:
- Windows 10 version 1809 (build 17763) or higher
- Visual Studio 2022 (for building)

### ExifDateFixer.Tests (Unit Tests)
```
ExifDateFixer.Tests/
├── Parsers/
│   ├── WhatsAppDateParserTests.cs
│   ├── SamsungDateParserTests.cs
│   └── WindowsPhoneDateParserTests.cs
├── Services/
│   └── ExifServiceTests.cs
└── ExifDateFixer.Tests.csproj          # References ExifDateFixer.Core
```

**Test Framework**: xUnit 2.9.3
**Test Count**: 49 tests (all passing)

## Build & Development Commands

### Prerequisites
- .NET 10.0 SDK (version 10.0.102 or compatible)
- **CRITICAL**: The project targets .NET 10.0, not .NET 8.0 or earlier versions
- **Note**: WinUI project can only be built on Windows with Visual Studio 2022

### Build Commands - TESTED AND VALIDATED

**ALWAYS run these commands from the repository root directory**.

#### Building on Linux/macOS

1. **Build Core Library**:
   ```bash
   dotnet build ExifDateFixer.Core/ExifDateFixer.Core.csproj
   ```

2. **Build CLI Application**:
   ```bash
   dotnet build ExifDateFixer.csproj
   ```
   - Builds to `bin/Debug/net10.0/ExifDateFixer.dll`
   - Also builds ExifDateFixer.Core automatically

3. **Build Tests**:
   ```bash
   dotnet build ExifDateFixer.Tests/ExifDateFixer.Tests.csproj
   ```

4. **Run Tests**:
   ```bash
   dotnet test ExifDateFixer.Tests/ExifDateFixer.Tests.csproj
   ```
   - All 49 tests should pass

#### Building on Windows

On Windows, you can build the entire solution including the WinUI GUI:

1. **Build Entire Solution**:
   ```bash
   dotnet build
   ```
   - Builds all projects: Core, CLI, WinUI, Tests
   - Note: May fail on Linux/macOS due to WinUI project

2. **Build WinUI GUI** (Windows only):
   ```bash
   dotnet build ExifDateFixer.WinUI/ExifDateFixer.WinUI.csproj
   ```
   - Or open in Visual Studio 2022 and build

### Running the Application

#### CLI Application:
   ```bash
   # Show help
   dotnet run --project ExifDateFixer.csproj -- --help
   
   # Scan current directory
   dotnet run --project ExifDateFixer.csproj -- .
   
   # Scan with options
   dotnet run --project ExifDateFixer.csproj -- /path/to/images -r -d
   
   # Dry run mode
   dotnet run --project ExifDateFixer.csproj -- /path/to/images --dry-run
   ```

#### GUI Application (Windows only):
   - Open `ExifDateFixer.sln` in Visual Studio 2022
   - Set `ExifDateFixer.WinUI` as startup project
   - Press F5 to run

### Testing

The project has 49 unit tests covering parsers and services:

```bash
# Run all tests
dotnet test ExifDateFixer.Tests/ExifDateFixer.Tests.csproj

# Run with verbose output
dotnet test ExifDateFixer.Tests/ExifDateFixer.Tests.csproj --verbosity normal
```

### Linting/Formatting
**NO LINTERS CONFIGURED** - No StyleCop, EditorConfig, or other code style tools are in use.

### Build Troubleshooting

**Common Issues & Solutions**:

1. **If build fails with "framework not found"**: Verify .NET 10.0 SDK is installed with `dotnet --version`

2. **If WinUI project fails on Linux/macOS**: This is expected. Build individual projects instead:
   - `dotnet build ExifDateFixer.Core/ExifDateFixer.Core.csproj`
   - `dotnet build ExifDateFixer.csproj`
   - `dotnet build ExifDateFixer.Tests/ExifDateFixer.Tests.csproj`

3. **If incremental build seems stale**: Run `dotnet clean` then build again

4. **After making code changes**: Simply run `dotnet build` - no special steps needed

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

### Cross-Platform Architecture

The solution uses a shared-core architecture to support multiple user interfaces:

```
┌─────────────────────────────────────┐
│   Platform-Specific UI Layers      │
├─────────────┬──────────┬────────────┤
│  CLI        │ WinUI 3  │  Future    │
│ (Any OS)    │(Windows) │(Mac/Linux) │
└─────────────┴──────────┴────────────┘
        │           │           │
        └───────────┼───────────┘
                    │
        ┌───────────▼────────────┐
        │  ExifDateFixer.Core    │
        │  (Business Logic)      │
        └────────────────────────┘
```

### Application Flow

#### Core Library (ExifDateFixer.Core)
1. **FileProcessor**: Scans directories, processes files, raises progress events
2. **ExifService** (namespace: `ExifDateFixer.Services`): Reads/writes EXIF metadata
3. **Parsers** (namespace: `ExifDateFixer.Parsers`): Extract dates from filenames

#### CLI Application (ExifDateFixer)
1. **Program.cs**: Entry point, parses command-line arguments
2. Creates FileProcessor and subscribes to progress events
3. Displays output to console

#### GUI Application (ExifDateFixer.WinUI)
1. **App.xaml/cs**: Application entry point
2. **MainWindow.xaml/cs**: Main window with folder picker, options, log viewer
3. Creates FileProcessor and subscribes to progress events
4. Updates UI with progress information

### Key Features

#### Dry-Run Mode
Both CLI and GUI support dry-run mode:
- CLI: `--dry-run` or `-d` flag
- GUI: "Dry run" checkbox
- Core: `ProcessFile(filePath, dryRun: true)`
- Returns `ProcessingStatus.WouldUpdate` instead of modifying files

#### Progress Reporting
FileProcessor raises `ProgressChanged` events:
- Event args include: FilePath, FileName, CurrentFile, TotalFiles, Result
- CLI subscribes to display console output
- GUI subscribes to update log viewer and progress bar

### Parser System (Extensible)
To add a new filename format parser:
1. Create a class in `ExifDateFixer.Core/Parsers/` implementing `IFilenameDateParser` interface
2. Use namespace `ExifDateFixer.Parsers`
3. Implement `Name` property and `TryParse(string filename, out DateTime dateTime)` method
4. Register parser in both `Program.cs` and `MainWindow.xaml.cs` in the parsers list
5. Add unit tests in `ExifDateFixer.Tests/Parsers/`
6. Build and test with sample filenames

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

## Common Development Tasks

### Add a new filename format parser
1. Create `MyFormatDateParser.cs` in `ExifDateFixer.Core/Parsers/` directory
2. Use namespace `ExifDateFixer.Parsers`
3. Implement `IFilenameDateParser` interface
4. Add regex pattern to extract date/time from filename
5. Register in parsers list in:
   - `Program.cs` (CLI)
   - `MainWindow.xaml.cs` (GUI)
6. Add unit tests in `ExifDateFixer.Tests/Parsers/`
7. Build and test with sample filenames

### Add a new file extension
1. Edit the supported extensions arrays in:
   - `Program.cs` (line ~49)
   - `MainWindow.xaml.cs` (line ~115)
2. Verify extension is supported by both MetadataExtractor and ExifLibNet

### Change output formatting
For CLI: Edit `Program.cs` (display and summary logic in ProcessFiles method)
For GUI: Edit `MainWindow.xaml.cs` (log formatting in ProcessFilesAsync method)

### Modify EXIF writing behavior
1. Edit `ExifDateFixer.Core/Services/ExifService.cs` method `WriteExifDate`
2. See ExifLibrary documentation for available ExifTag values

### Update version number
1. Edit version properties in:
   - `ExifDateFixer.csproj`: `Version`, `AssemblyVersion`, `FileVersion`
   - `ExifDateFixer.Core/ExifDateFixer.Core.csproj`: `Version`
   - `ExifDateFixer.WinUI/ExifDateFixer.WinUI.csproj`: `Version`
2. Rebuild the project

### Add a GUI for another platform
1. Create new project (e.g., `ExifDateFixer.Avalonia` for Linux/macOS)
2. Add project reference to `ExifDateFixer.Core`
3. Implement UI using platform's framework
4. Subscribe to FileProcessor.ProgressChanged events
5. Use FileProcessor.ProcessFiles or ProcessFile methods

## Important Notes for Agents

1. **Architecture**: This is a multi-project solution. Always reference ExifDateFixer.Core for business logic.

2. **Cross-platform builds**: 
   - CLI builds on all platforms
   - WinUI only builds on Windows
   - Build individual projects on Linux/macOS

3. **Testing**: 49 unit tests exist in ExifDateFixer.Tests. Always run them after changes.

4. **Dry-run mode**: Both CLI and GUI support dry-run. Test this feature when making changes.

5. **Progress events**: GUI and future UIs rely on FileProcessor.ProgressChanged events. Preserve this functionality.

6. **Case-insensitive extensions**: File extension matching is case-insensitive (FileProcessor.cs handles this).

7. **Parser order matters**: Parsers are tried in order. First match wins.

8. **No database or state**: This is a stateless tool. Each run is independent.

9. **Error handling**: The application handles exceptions gracefully and continues processing other files.

10. **Namespace organization**: 
   - `ExifDateFixer` - CLI application and shared types
   - `ExifDateFixer.Parsers` - All parsers
   - `ExifDateFixer.Services` - Services (ExifService)
   - `ExifDateFixer.WinUI` - Windows GUI

11. **License**: This project is licensed under GPL-3.0. Always respect GPL-3.0 license terms when making changes.
