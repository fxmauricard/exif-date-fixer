# Building on Non-Windows Systems

This solution contains a WinUI 3 project (`ExifDateFixer.WinUI`) that can only be built on Windows with Visual Studio 2022.

## Building on Linux/macOS

When building on Linux or macOS, you can build specific projects:

```bash
# Build the Core library
dotnet build ExifDateFixer.Core/ExifDateFixer.Core.csproj

# Build the CLI application
dotnet build ExifDateFixer.csproj

# Run tests
dotnet test ExifDateFixer.Tests/ExifDateFixer.Tests.csproj
```

Or build the solution excluding the WinUI project:

```bash
# The WinUI project will fail to restore/build on non-Windows systems
# But you can build the other projects individually as shown above
```

## Building on Windows

On Windows with Visual Studio 2022, you can build the entire solution:

```bash
# Build everything including the WinUI GUI
dotnet build

# Or use Visual Studio to build and run the GUI
```

The WinUI project requires:
- Windows 10 version 1809 (build 17763) or higher
- .NET 10.0 SDK
- Windows App SDK 1.6 or higher
