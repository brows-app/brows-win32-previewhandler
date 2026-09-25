using Brows.Win32.PlatformInvoke;
using System.Runtime.InteropServices;

namespace Brows.Win32.InteropServices.ComTypes;

[Guid(IID.IInitializeWithItem)]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IInitializeWithItem {
    [PreserveSig]
    HRESULT Initialize(
        [In] IShellItem psi,
        [In] STGM grfMode);
}
