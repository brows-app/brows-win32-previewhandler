using System.Runtime.InteropServices;
using HWND = System.IntPtr;
using LPARAM = System.Int64;
using LRESULT = System.Int64;
using UINT = System.UInt32;
using WPARAM = System.UInt32;

namespace Brows.Win32.PlatformInvoke;

internal static class user32 {
    [DllImport("user32.dll", SetLastError = true, PreserveSig = true)]
    public static extern bool GetClientRect([In] nint hWnd, [In, Out] ref RECT lpRect);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true, PreserveSig = true)]
    public static extern nint CreateWindowExW(
      [In] uint dwExStyle,
      [In, Optional, MarshalAs(UnmanagedType.LPWStr)] string lpClassName,
      [In, Optional, MarshalAs(UnmanagedType.LPWStr)] string lpWindowName,
      [In] uint dwStyle,
      [In] int X,
      [In] int Y,
      [In] int nWidth,
      [In] int nHeight,
      [In, Optional] nint hWndParent,
      [In, Optional] nint hMenu,
      [In, Optional] nint hInstance,
      [In, Optional] nint lpParam);

    [DllImport("user32.dll", SetLastError = true, PreserveSig = true)]
    public static extern bool DestroyWindow([In] nint hWnd);

    [DllImport("user32.dll", SetLastError = true, PreserveSig = true)]
    public static extern LRESULT DefWindowProcW(
        [In] HWND hWnd,
        [In] UINT Msg,
        [In] WPARAM wParam,
        [In] LPARAM lParam);
}
