using Brows.Win32.PlatformInvoke;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Brows.Windows.Controls;

internal sealed partial class PreviewHwndHost : HwndHost {
    protected sealed override HandleRef BuildWindowCore(HandleRef hwndParent) {
        var hwnd = user32.CreateWindowExW(
            dwExStyle: 0,
            lpClassName: "Static",
            lpWindowName: "",
            dwStyle: 0x10000000 | 0x40000000 | 0x02000000,
            X: 0,
            Y: 0,
            nWidth: 0,
            nHeight: 0,
            hWndParent: hwndParent.Handle,
            hMenu: (IntPtr)0x00000002,
            hInstance: IntPtr.Zero,
            IntPtr.Zero);
        if (hwnd == IntPtr.Zero) {
            throw new Win32Exception();
        }
        return new HandleRef(this, hwnd);
    }

    protected sealed override void DestroyWindowCore(HandleRef hwnd) {
        var success = user32.DestroyWindow(hwnd.Handle);
        if (success == false) {
            throw new Win32Exception();
        }
    }

    public PreviewHwndHost() {
        InitializeComponent();
    }
}
