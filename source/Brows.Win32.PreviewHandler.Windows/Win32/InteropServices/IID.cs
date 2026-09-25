namespace Brows.Win32.InteropServices;

internal static class IID {
    public const string IInitializeWithFile = "b7d14566-0509-4cce-a71f-0a554233bd9b";
    public const string IInitializeWithItem = "7f73be3f-fb79-493c-a6c7-7ee14e245841";
    public const string IInitializeWithStream = "b824b49d-22ac-4161-ac8a-9916e8fa3f7f";
    public const string IShellItem = "43826D1E-E718-42EE-BC55-A1E261C37BFE";
    public const string IObjectWithSite = "fc4801a3-2ba9-11cf-a229-00aa003d7352";
    public const string IPreviewHandler = "8895b1c6-b41f-4c1c-a562-0d564250836f";
    public const string IPreviewHandlerFrame = "fec87aaf-35f9-447a-adb7-20234491401a";
    public const string IPreviewHandlerVisuals = "196bf9a5-b346-4ef0-aa1e-5dcdb76768b1";

    public static class Managed {
        public static readonly Guid IShellItem = new Guid(IID.IShellItem);
        public static readonly Guid IPreviewHandler = new Guid(IID.IPreviewHandler);
    }
}
