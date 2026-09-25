using System.Runtime.InteropServices;

namespace Brows.Win32.PlatformInvoke;

[StructLayout(LayoutKind.Sequential)]
internal struct RECT {
    public int left;
    public int top;
    public int right;
    public int bottom;
}
