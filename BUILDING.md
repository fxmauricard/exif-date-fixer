# Building EXIF Date Fixer

This solution contains a WinUI 3 project (`ExifDateFixer.WinUI`) that can only be built on Windows with Visual Studio 2022. However, the Avalonia GUI (`ExifDateFixer.Avalonia`) builds on all platforms.

## Building on Linux/macOS

When building on Linux or macOS, you can build these projects:

```bash
# Build the Core library
dotnet build ExifDateFixer.Core/ExifDateFixer.Core.csproj

# Build the CLI application
dotnet build ExifDateFixer.csproj

# Build the Avalonia cross-platform GUI
dotnet build ExifDateFixer.Avalonia/ExifDateFixer.Avalonia.csproj

# Run tests
dotnet test ExifDateFixer.Tests/ExifDateFixer.Tests.csproj
```

**Note**: Building the entire solution (`dotnet build`) will fail on Linux/macOS because the WinUI project requires Windows-specific SDKs. Build individual projects as shown above.

## Building on Windows

On Windows with Visual Studio 2025, you can build the entire solution:

```bash
# Build everything including both GUIs (WinUI and Avalonia)
dotnet build

# Or use Visual Studio to build and run the GUIs
```

The WinUI project requires:
- Windows 10 version 1809 (build 17763) or higher
- .NET 10.0 SDK
- Windows App SDK 1.6 or higher

## Running the Applications

### CLI (All Platforms)
```bash
dotnet run --project ExifDateFixer.csproj -- /path/to/images -r --dry-run
```

### Avalonia GUI (All Platforms)
```bash
dotnet run --project ExifDateFixer.Avalonia/ExifDateFixer.Avalonia.csproj
```

### WinUI GUI (Windows Only)
Open the solution in Visual Studio 2025, set `ExifDateFixer.WinUI` as startup project, and press F5.
