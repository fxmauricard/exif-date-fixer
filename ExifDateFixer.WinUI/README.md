# EXIF Date Fixer - WinUI 3 GUI

This is the Windows GUI application for EXIF Date Fixer, built with WinUI 3 and the Windows App SDK.

## Requirements

- Windows 10 version 1809 (build 17763) or higher
- .NET 10.0 or higher
- Visual Studio 2022 (for building)
- Windows App SDK 1.6 or higher

## Features

- **Modern Windows 11 UI**: Uses WinUI 3 controls and Fluent Design
- **Folder Selection**: Browse and select folders to process
- **Recursive Scanning**: Option to scan subdirectories
- **Dry Run Mode**: Preview changes without modifying files
- **Real-time Progress**: Live log display with processing status
- **Cross-platform Architecture**: Shares business logic with CLI via ExifDateFixer.Core

## Building

### Using Visual Studio 2022

1. Open `ExifDateFixer.sln` in Visual Studio 2022
2. Set `ExifDateFixer.WinUI` as the startup project
3. Select your target platform (x64, x86, or ARM64)
4. Build and run (F5)

### Using Command Line (on Windows)

```powershell
dotnet build ExifDateFixer.WinUI/ExifDateFixer.WinUI.csproj -c Release
```

## Running

After building, you can run the application from:
- Visual Studio (F5)
- The built executable: `ExifDateFixer.WinUI\bin\{Platform}\{Configuration}\net10.0-windows10.0.19041.0\ExifDateFixer.WinUI.exe`

## Usage

1. Click "Browse..." to select a folder containing images
2. Check "Scan subdirectories recursively" if you want to process subfolders
3. Check "Dry run" to preview changes without modifying files
4. Click "Start Processing" to begin
5. View real-time progress and results in the log viewer

## Architecture

The WinUI project follows a clean architecture pattern:

- **MainWindow.xaml**: UI definition using XAML
- **MainWindow.xaml.cs**: Code-behind with UI logic and event handlers
- **ExifDateFixer.Core**: Shared business logic (referenced as project dependency)

This architecture allows for easy extension to other platforms (macOS, Linux) in the future by creating platform-specific UI projects that reference the same Core library.

## Supported File Formats

- JPEG (.jpg, .jpeg)
- HEIF (.heic, .heif)

## Supported Filename Patterns

- **WhatsApp**: IMG-20230115-WA0001.jpg
- **Samsung**: 20230115_123045.jpg
- **Windows Phone**: WP_20230115_12_30_45_Pro.jpg
