using Brows.Win32.PlatformInvoke;
using System.Runtime.InteropServices;

namespace Brows.Win32.InteropServices.ComTypes;

[Guid(IID.IObjectWithSite)]
[ComImport]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
interface IObjectWithSite {
    [PreserveSig]
    HRESULT SetSite(
        [In, MarshalAs(UnmanagedType.IUnknown)] object pUnkSite);

    [PreserveSig]
    HRESULT GetSite(
        [In] ref Guid riid,
        [Out, MarshalAs(UnmanagedType.IUnknown, IidParameterIndex = 0)] out object ppvSite);
}
