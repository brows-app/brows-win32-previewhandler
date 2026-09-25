using Brows.Win32.PlatformInvoke;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Brows.Win32.InteropServices.ComTypes;

[Guid(IID.IInitializeWithStream)]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
internal interface IInitializeWithStream {
    [PreserveSig] HRESULT Initialize(
        IStream pstream,
        [MarshalAs(UnmanagedType.U4)] STGM grfMode);
}
