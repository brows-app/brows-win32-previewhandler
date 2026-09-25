using Brows.Win32.PlatformInvoke;
using System.Runtime.InteropServices;

namespace Brows.Win32.InteropServices.ComTypes;

[Guid(IID.IInitializeWithFile)]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IInitializeWithFile {
    [PreserveSig]
    HRESULT Initialize(
        [In][MarshalAs(UnmanagedType.LPWStr)] string pszFilePath,
        [In][MarshalAs(UnmanagedType.U4)] STGM grfMode);
}
