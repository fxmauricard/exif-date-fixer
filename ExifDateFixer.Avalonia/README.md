# EXIF Date Fixer - Cross-Platform GUI (Avalonia)

This is the cross-platform GUI application for EXIF Date Fixer, built with Avalonia UI.

## Requirements

- .NET 10.0 or higher
- Works on:
  - **Windows** (7, 8, 10, 11)
  - **macOS** (10.13 High Sierra or higher)
  - **Linux** (most distributions with X11 or Wayland)

## Features

- **Cross-Platform**: Single codebase runs on Windows, macOS, and Linux
- **Modern UI**: Clean, Fluent Design-inspired interface
- **Folder Selection**: Browse and select folders to process
- **Recursive Scanning**: Option to scan subdirectories
- **Dry Run Mode**: Preview changes without modifying files
- **Real-time Progress**: Live log display with processing status
- **Shared Business Logic**: Uses ExifDateFixer.Core library

## Building

### Using Command Line

```bash
dotnet build ExifDateFixer.Avalonia/ExifDateFixer.Avalonia.csproj -c Release
```

### Using Visual Studio / Visual Studio Code / Rider

1. Open `ExifDateFixer.sln`
2. Set `ExifDateFixer.Avalonia` as the startup project
3. Build and run (F5)

## Running

### From Command Line

```bash
dotnet run --project ExifDateFixer.Avalonia/ExifDateFixer.Avalonia.csproj
```

### From Built Executable

After building, run the executable from:
- `ExifDateFixer.Avalonia/bin/Release/net10.0/ExifDateFixer.Avalonia` (Linux/macOS)
- `ExifDateFixer.Avalonia\bin\Release\net10.0\ExifDateFixer.Avalonia.exe` (Windows)

## Usage

1. Launch the ExifDateFixer Avalonia application
2. Click "Browse..." to select a folder containing images
3. Check options:
   - "Scan subdirectories recursively" to include subfolders
   - "Dry run" to preview changes without modifying files
4. Click "Start Processing"
5. View real-time progress and results in the log viewer

## Architecture

The Avalonia project follows the same clean architecture pattern as the WinUI version:

- **MainWindow.axaml**: UI definition using AXAML (Avalonia XAML)
- **MainWindow.axaml.cs**: Code-behind with UI logic and event handlers
- **ExifDateFixer.Core**: Shared business logic (referenced as project dependency)

This architecture allows for maximum code reuse:
- Same Core library used by CLI, WinUI, and Avalonia
- Easy to maintain and extend
- Consistent behavior across all platforms

## Supported File Formats

- JPEG (.jpg, .jpeg)
- HEIF (.heic, .heif)

## Supported Filename Patterns

- **WhatsApp**: IMG-20230115-WA0001.jpg
- **Samsung**: 20230115_123045.jpg
- **Windows Phone**: WP_20230115_12_30_45_Pro.jpg

## Technology Stack

- **Avalonia UI 11.3.11**: Cross-platform .NET UI framework
- **ExifDateFixer.Core**: Shared business logic library
- **.NET 10.0**: Runtime and SDK

## Differences from WinUI Version

The Avalonia GUI provides the same functionality as the WinUI version but:
- Works on Windows, macOS, and Linux (not just Windows)
- Uses Avalonia's cross-platform file picker
- Uses Avalonia's theming system (adapts to system theme)
- Slightly different styling but same UX

Both GUIs share 100% of the business logic via ExifDateFixer.Core.
