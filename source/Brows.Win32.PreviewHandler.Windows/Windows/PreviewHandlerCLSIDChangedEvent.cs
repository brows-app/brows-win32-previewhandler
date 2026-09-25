using System.Windows;

namespace Brows.Windows;

/// <summary>
/// Handles notification that the preview handler for a file extension has been resolved.
/// </summary>
/// <param name="sender">The object that raised the event.</param>
/// <param name="e">The resolved extension and preview handler class identifier.</param>
public delegate void PreviewHandlerCLSIDChangedEventHandler(object sender, PreviewHandlerCLSIDChangedEventArgs e);

/// <summary>
/// Provides the resolved preview handler class identifier and allows it to be overridden.
/// </summary>
public sealed class PreviewHandlerCLSIDChangedEventArgs : RoutedEventArgs {
    /// <summary>
    /// Gets the file extension for which the preview handler was resolved.
    /// </summary>
    public string Extension { get; }

    /// <summary>
    /// Gets the class identifier of the resolved preview handler.
    /// </summary>
    public Guid CLSID { get; }

    /// <summary>
    /// Gets or sets an alternate preview handler class identifier.
    /// Leave this value unset to use <see cref="CLSID"/>.
    /// </summary>
    public Guid? Override { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PreviewHandlerCLSIDChangedEventArgs"/> class.
    /// </summary>
    /// <param name="extension">The file extension associated with the preview handler.</param>
    /// <param name="clsid">The resolved preview handler class identifier.</param>
    /// <param name="routedEvent">The routed event associated with these event arguments.</param>
    public PreviewHandlerCLSIDChangedEventArgs(string extension, Guid clsid, RoutedEvent routedEvent) : base(routedEvent) {
        Extension = extension;
        CLSID = clsid;
    }
}
