using Brows.Win32.PlatformInvoke;
using System.Runtime.InteropServices;

namespace Brows.Win32.InteropServices.ComTypes;

[Guid(IID.IPreviewHandlerVisuals)]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IPreviewHandlerVisuals {
    [PreserveSig]
    HRESULT SetBackgroundColor(uint color);

    [PreserveSig]
    HRESULT SetFont(ref LOGFONTW plf);

    [PreserveSig]
    HRESULT SetTextColor(uint color);
}
