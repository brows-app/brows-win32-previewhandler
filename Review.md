# Code Review

## Findings

| # | Severity | Location | Finding |
|---|---|---|---|
| 1 | High (fixed) | `source/Brows.Win32.Windows.PreviewHandler/Win32/InteropServices/PreviewHandlerWrapper.cs:70-91` | `CoCreateInstance` returns an owning interface pointer in `ppv`. After `Marshal.GetUniqueObjectForIUnknown(ppv)` succeeds, the original pointer must also be released; otherwise each activation leaks a COM reference. `Factory` now releases `ppv` in a `finally`, including when activation/conversion falls back to `Activator.CreateInstance`, while retaining the RCW reference. |
| 2 | High (fixed) | `source/Brows.Win32.Windows.PreviewHandler/Windows/Controls/PreviewHandler.xaml.cs:105-146` | Refreshes now create their cancellation source before the debounce delay, cancel pending delays as well as preview starts, and only clear the shared field or update success/loading state when their token source is still current. This prevents an older refresh from clearing a newer refresh's cancellation handle or overwriting its UI state. |
| 3 | Medium (fixed) | `source/Brows.Win32.Windows.PreviewHandler/Windows/Controls/PreviewHandler.xaml.cs` and `source/Brows.Win32.Windows.PreviewHandler/Win32/InteropServices/PreviewWorker.cs` | Disabling the control or changing/clearing the source now cancels in-flight work, clears the preview state, and unloads the worker. Worker requests are invalidated before queued unloads, and unsuccessful starts (empty input, unavailable window/rect, missing handler CLSID, or CLSID lookup failure) unload the prior handler rather than leaving stale content visible. |
| 4 | Medium (needs verification) | `Directory.Build.props:10`, `:15` | `PackageProjectUrl` and `RepositoryUrl` have been changed from the STAThreadPool repository to `brows-app/brows-win32-previewhandler`. The configured GitHub URLs return 404 to unauthenticated requests, and this checkout has no Git remote; confirm these are the canonical URLs (the repository may be private). |
| 5 | Low (fixed) | `README.md`, `Directory.Build.props:6`, `:11` | The repository README now documents the preview-handler package and is included as the NuGet package readme. Package metadata now includes a product description and relevant tags. |
| 6 | Low (fixed) | `source/Brows.Win32.Windows.PreviewHandler/Properties/AssemblyInfo.cs:6-7` | The `brows:` XAML namespace maps both `Brows.Windows` (where the public CLSID event types live) and `Brows.Windows.Controls` (where `PreviewHandler` lives). |
| 7 | Low (fixed) | `Directory.Build.props:23` | `ArtifactsPath` is now rooted at `$(MSBuildThisFileDirectory)out`, so it resolves to the repository's `out` directory independently of project depth. |
| 8 | Low (fixed) | `source/Directory.Build.props:5`, `source/Brows.Win32.Windows.PreviewHandler/Windows/Controls/PreviewHandler.xaml.cs`, `source/Brows.Win32.Windows.PreviewHandler/Windows/PreviewHandlerCLSIDChangedEvent.cs` | XML documentation has been added to the public control and CLSID event API; the build no longer reports CS1591 missing-documentation warnings. |

## Assessment of findings 1-3

- **#1 — Agree; fixed.** `Marshal.GetUniqueObjectForIUnknown` adds its own reference, so `Factory` now releases the `ppv` reference in a `finally`. This also releases the pointer if wrapper creation fails and the code falls back to `Activator.CreateInstance`.
- **#2 — Agree; fixed.** Each refresh now owns a local cancellation source that is installed before the debounce delay. Completion only clears the shared source and loading state if the field still references that refresh, and success is applied only while its source remains current. Cancellation during the delay is handled without logging it as an error.
- **#3 — Agree; fixed.** Disabling the control and changing/clearing its source now cancel current work, clear success/loading state, and queue worker unload. `PreviewWorker` invalidates pending starts when unloading, and failed starts also unload the previous handler so empty/unsupported input or unavailable HWND/rect cannot leave stale preview content attached.

## Project file

`Brows.Win32.Windows.PreviewHandler.csproj` only declares the two package references. Everything else comes from the shared `Directory.Build.props`/`Directory.Packages.props` files. The project builds and packs for all four TFMs (`net462`, `net48`, `net8.0-windows`, `net10.0-windows`), and produces `.nupkg` + `.snupkg` with the project README, package metadata, per-TFM dependencies, and WPF framework references. Finding #4 is the only item still requiring confirmation.
