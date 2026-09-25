using Brows.Win32.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace Brows.Win32.PlatformInvoke;

internal static class shell32 {
    public const int MAX_PATH = 260;

    [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = true)]
    public static extern HRESULT SHCreateItemFromParsingName(
        [In] string pszPath,
        [In, Optional] nint pbc,
        [In] ref Guid riid,
        [Out][MarshalAs(UnmanagedType.Interface, IidParameterIndex = 2)] out IShellItem ppv);
}
