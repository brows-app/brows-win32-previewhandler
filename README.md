# Brows.Win32.Windows.PreviewHandler

A WPF control that displays files using the Windows shell preview handler registered for each file
type.

The control hosts the native preview handler in a child window and runs its COM operations on a
single-threaded apartment (STA) worker. It uses handlers installed and registered by Windows or
other applications; this package does not include or register handlers.

## Installation

```powershell
dotnet add package Brows.Win32.Windows.PreviewHandler
```

The package targets `net462`, `net48`, `net8.0-windows`, and `net10.0-windows`. It requires Windows
and WPF. Applications targeting .NET 8 or later should use a Windows TFM, such as `net10.0-windows`.

## Usage

Add the Brows XAML namespace and bind `PreviewHandlerSource` to a file path:

```xml
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:brows="http://schemas.brows.app/winfx/2026/xaml/presentation">
  <Grid>
    <brows:PreviewHandler PreviewHandlerSource="{Binding FilePath}" />
  </Grid>
</Window>
```

The file type must have a Windows shell preview handler registered on the machine. The handler's
availability can depend on the installed applications and the process architecture.

## Control properties and events

| Member | Description |
| --- | --- |
| `PreviewHandlerSource` | File path to preview. Set to `null` or an empty string to clear the preview. |
| `PreviewHandlerEnabled` | Enables or disables previewing. Defaults to `true`; disabling unloads the active handler. |
| `PreviewHandlerRefresh` | Set to `true` to request a refresh; the control resets it to `false`. |
| `PreviewHandlerLoading` | Read-only indicator that a preview is loading. |
| `PreviewHandlerSuccess` | Read-only indicator that the current file was successfully previewed. |
| `PreviewHandlerCLSIDChanged` | Raised when the handler class identifier is resolved. Set `e.Override` to use a different registered handler. |
| `PreviewHandlerLoadingChanged` | Raised when `PreviewHandlerLoading` changes. |
| `PreviewHandlerSuccessChanged` | Raised when `PreviewHandlerSuccess` changes. |

To override the resolved handler, handle `PreviewHandlerCLSIDChanged` and assign the CLSID of
another registered preview handler to `e.Override`.

## License

[MIT](LICENSE)
