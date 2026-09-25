namespace Brows.Win32;

internal delegate void PreviewWorkerCLSIDChangedEventHandler(object sender, PreviewWorkerCLSIDChangedEventArgs e);

internal sealed class PreviewWorkerCLSIDChangedEventArgs : EventArgs {
    public string Extension { get; }
    public Guid CLSID { get; }
    public Guid? Override { get; set; }

    public PreviewWorkerCLSIDChangedEventArgs(string extension, Guid clsid) {
        Extension = extension;
        CLSID = clsid;
    }
}
