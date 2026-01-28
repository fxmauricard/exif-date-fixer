# WinUI 3 GUI Visual Design Description

Since the WinUI 3 GUI can only be run on Windows, here's a detailed description of the user interface:

## Window Layout

The main window is divided into 4 sections:

### 1. Header Section (Top)
- **Background**: Card background color (light theme: white, dark theme: dark gray)
- **Icon**: Large folder/image icon (Segoe MDL2 Assets glyph)
- **Title**: "EXIF Date Fixer" in large, bold text (TitleTextBlockStyle)
- **Version**: "Version 1.0.0" in smaller, secondary text below the title

### 2. Settings Panel (Below Header)
- **Section Title**: "Settings" in subtitle style
- **Path Selection**:
  - Text box displaying selected folder path (read-only, placeholder: "Select a folder to scan for images...")
  - "Browse..." button (Accent style - blue button) to open folder picker
- **Options** (Checkboxes):
  - ☑ "Scan subdirectories recursively" (checked by default)
  - ☐ "Dry run (preview changes without modifying files)" (unchecked by default)
- **Action Buttons**:
  - "Start Processing" (Accent button - blue)
  - "Clear Log" (Standard button)

### 3. Log Viewer (Center - Takes Most Space)
- **Section Title**: "Processing Log"
- **Log Display**:
  - Bordered container with rounded corners
  - Background: Layer fill color (slightly different from window background)
  - Scrollable text area with vertical scrollbar
  - Monospace font (Consolas) for log entries
  - Text is selectable for copying
  - Auto-scrolls to show latest entries

### 4. Status Bar (Bottom)
- **Background**: Card background color
- **Left Side**: Status text (e.g., "Ready", "Processing 5/150: IMG-20230115-WA0001.jpg")
- **Right Side**: Progress bar (200px wide, appears during processing)

## User Interaction Flow

1. **Launch**: App opens with empty path textbox and "Ready" status
2. **Browse**: User clicks "Browse...", selects folder via Windows folder picker
3. **Configure**: User checks/unchecks options as needed
4. **Start**: User clicks "Start Processing"
   - Buttons become disabled
   - Progress bar appears
   - Log starts filling with processing information
   - Status updates with current file being processed
5. **Complete**: When done, buttons re-enable, progress bar shows 100%, summary displayed in log

## Log Format Example

```
EXIF Date Fixer - Scanning: C:\Photos (DRY RUN)
Recursive: Yes
Dry Run: Yes
Supported extensions: .jpg, .jpeg, .heic, .heif

Finding files...
Found 150 files to process...

[1/150] IMG-20230115-WA0001.jpg
  → Would add EXIF date 2023-01-15 00:00:00

[2/150] photo.jpg
  - Could not parse date from filename

[3/150] vacation.jpg
  - EXIF date already exists

Summary:
- Total files: 150
- Would update: 45
- Skipped (has date): 80
- Skipped (no date in filename): 25
- Errors: 0
```

## Design Principles

1. **Fluent Design**: Uses WinUI 3's built-in styles and controls
2. **Responsive**: Adapts to window resizing (log viewer expands/contracts)
3. **Accessible**: Proper contrast ratios, keyboard navigation support
4. **Modern**: Follows Windows 11 design guidelines
5. **Clear Feedback**: Real-time progress and detailed logging

## Visual Style

- **Spacing**: Consistent padding and margins (8px, 12px, 16px, 24px)
- **Rounded Corners**: Modern rounded corners on containers (4px radius)
- **Typography**: Uses Windows typography scale (Title, Subtitle, Body, Caption)
- **Colors**: Adapts to system theme (Light/Dark mode)
- **Icons**: Uses Segoe MDL2 Assets font for status symbols (✓, ✗, -, →)

## Additional Features Not Yet Implemented

Possible future enhancements:
- Drag-and-drop folder selection
- Recent folders list
- Export log to file
- Statistics dashboard
- Settings persistence
- Custom parser configuration UI
