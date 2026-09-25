using Brows.Win32.PlatformInvoke;
using System.Runtime.InteropServices;

namespace Brows.Win32.InteropServices.ComTypes;

[Guid(IID.IPreviewHandler)]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IPreviewHandler {
    [PreserveSig]
    HRESULT SetWindow(nint hwnd, ref RECT prc);

    [PreserveSig]
    HRESULT SetRect(ref RECT prc);

    [PreserveSig]
    HRESULT DoPreview();

    [PreserveSig]
    HRESULT Unload();

    [PreserveSig]
    HRESULT SetFocus();

    [PreserveSig]
    HRESULT QueryFocus(out nint phwnd);
}
