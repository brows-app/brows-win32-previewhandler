using System;
using System.Runtime.InteropServices;

namespace Brows.Win32.PlatformInvoke;

internal static class ole32 {
    [DllImport("ole32.dll", PreserveSig = true)]
    public static extern HRESULT CoCreateInstance(
        [In] ref Guid rclsid,
        IntPtr pUnkOuter,
        CLSCTX dwClsContext,
        [In] ref Guid riid,
        [Out] out nint ppv);

    [DllImport("ole32.dll", PreserveSig = true)]
    public static extern void CoTaskMemFree([In, Optional] nint pv);
}
