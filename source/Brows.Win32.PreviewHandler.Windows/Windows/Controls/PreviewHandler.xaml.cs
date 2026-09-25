using Brows.Win32;
using Brows.Win32.PlatformInvoke;
using Domore.Logs;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Brows.Windows.Controls;

/// <summary>
/// Displays a file preview using the Windows shell preview handler registered for the file type.
/// </summary>
partial class PreviewHandler {
    private static readonly ILog Log = Logging.For(typeof(PreviewHandler));

    private PreviewWorker Worker;
    private CancellationTokenSource TokenSource;

    private bool IsCurrent(CancellationTokenSource tokenSource) =>
        ReferenceEquals(TokenSource, tokenSource) &&
        tokenSource.IsCancellationRequested == false;

    private void TryCancel() {
        var tokenSource = TokenSource;
        if (tokenSource is not null) {
            try {
                tokenSource.Cancel();
            }
            catch {
            }
        }
    }

    private static async Task UnloadWorker(PreviewWorker worker) {
        try {
            await worker.Unload(CancellationToken.None);
        }
        catch (Exception ex) {
            if (Log.Warn()) {
                Log.Warn(ex);
            }
        }
    }

    private void ClearPreview() {
        TryCancel();
        var worker = Worker;
        if (worker is not null) {
            _ = UnloadWorker(worker);
        }
        PreviewHandlerSuccess = false;
        PreviewHandlerLoading = false;
    }

    private static readonly DependencyPropertyKey PreviewHandlerSuccessPropertyKey =
        DependencyProperty.RegisterReadOnly(
            name: nameof(PreviewHandlerSuccess),
            propertyType: typeof(bool),
            ownerType: typeof(PreviewHandler),
            typeMetadata: new PropertyMetadata(
                defaultValue: false,
                propertyChangedCallback: (s, e) => {
                    if (s is PreviewHandler self) {
                        self.OnPreviewHandlerSuccessChanged(e);
                    }
                }));

    private static readonly DependencyPropertyKey PreviewHandlerLoadingPropertyKey =
        DependencyProperty.RegisterReadOnly(
            name: nameof(PreviewHandlerLoading),
            propertyType: typeof(bool),
            ownerType: typeof(PreviewHandler),
            typeMetadata: new PropertyMetadata(
                defaultValue: false,
                propertyChangedCallback: (s, e) => {
                    if (s is PreviewHandler self) {
                        self.OnPreviewHandlerLoadingChanged(e);
                    }
                }));

    private void Worker_CLSIDChanged(object sender, PreviewWorkerCLSIDChangedEventArgs e) {
        if (e is not null) {
            var a = new PreviewHandlerCLSIDChangedEventArgs(e.Extension, e.CLSID, PreviewHandlerCLSIDChangedEvent);
            RaiseEvent(a);
            e.Override = a.Override;
        }
    }

    private async void This_Unloaded(object sender, RoutedEventArgs e) {
        TryCancel();
        var
        worker = Worker;
        Worker = null;
        if (worker is not null) {
            worker.CLSIDChanged -= Worker_CLSIDChanged;
            await UnloadWorker(worker);
        }
    }

    private void This_Loaded(object sender, RoutedEventArgs e) {
        Worker = new();
        Worker.CLSIDChanged += Worker_CLSIDChanged;
    }

    private RECT? Rect() {
        var host = GetTemplateChild("PART_HwndHost") as PreviewHwndHost;
        if (host is null) {
            return null;
        }
        var source = PresentationSource.FromVisual(host);
        if (source is null) {
            return null;
        }
        var transform = source.CompositionTarget.TransformToDevice;
        var physical = (Size)transform.Transform((Vector)host.RenderSize);
        return new RECT {
            left = 0,
            top = 0,
            right = (int)physical.Width,
            bottom = (int)physical.Height
        };
    }

    private async void Refresh() {
        TryCancel();
        var source = PreviewHandlerSource;
        if (PreviewHandlerEnabled == false || string.IsNullOrEmpty(source)) {
            ClearPreview();
            return;
        }
        using (var tokenSource = TokenSource = new()) {
            var ct = tokenSource.Token;
            try {
                await Task.Delay(250, ct);
                if (source != PreviewHandlerSource || !IsCurrent(tokenSource)) {
                    return;
                }
                var worker = Worker;
                if (worker is not null) {
                    PreviewHandlerSuccess = false;
                    PreviewHandlerLoading = true;
                    if (!IsCurrent(tokenSource)) {
                        return;
                    }
                    var host = GetTemplateChild("PART_HwndHost") as PreviewHwndHost;
                    var success = await worker.Start(
                        file: source,
                        hwnd: host?.Handle,
                        rect: Rect(),
                        cancellationToken: ct);
                    if (IsCurrent(tokenSource)) {
                        PreviewHandlerSuccess = success;
                    }
                }
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) {
            }
            catch (Exception ex) {
                if (Log.Warn()) {
                    Log.Warn(ex);
                }
            }
            finally {
                if (ReferenceEquals(TokenSource, tokenSource)) {
                    TokenSource = null;
                    PreviewHandlerLoading = false;
                }
            }
        }
    }

    private void HwndHost_Loaded(object sender, RoutedEventArgs e) {
        Refresh();
    }

    private async void HwndHost_SizeChanged(object sender, SizeChangedEventArgs e) {
        if (PreviewHandlerEnabled) {
            var worker = Worker;
            if (worker is not null) {
                try {
                    await worker.Change(Rect(), CancellationToken.None);
                }
                catch (Exception ex) {
                    if (Log.Info()) {
                        Log.Info(ex);
                    }
                }
            }
        }
    }

    private void OnPreviewHandlerSourceChanged(DependencyPropertyChangedEventArgs e) {
        ClearPreview();
        if (PreviewHandlerEnabled && string.IsNullOrEmpty(PreviewHandlerSource) == false) {
            Refresh();
        }
    }

    private void OnPreviewHandlerRefreshChanged(DependencyPropertyChangedEventArgs e) {
        if (PreviewHandlerRefresh) {
            PreviewHandlerRefresh = false;
            Refresh();
        }
    }

    private void OnPreviewHandlerEnabledChanged(DependencyPropertyChangedEventArgs e) {
        if (PreviewHandlerEnabled == false) {
            ClearPreview();
        }
        else {
            Refresh();
        }
    }

    private void OnPreviewHandlerLoadingChanged(DependencyPropertyChangedEventArgs e) {
        var a = new RoutedEventArgs(PreviewHandlerLoadingChangedEvent);
        RaiseEvent(a);
    }

    private void OnPreviewHandlerSuccessChanged(DependencyPropertyChangedEventArgs e) {
        var a = new RoutedEventArgs(PreviewHandlerSuccessChangedEvent);
        RaiseEvent(a);
    }

    /// <summary>
    /// Identifies the routed event raised when the preview handler class identifier is resolved.
    /// </summary>
    public static readonly RoutedEvent PreviewHandlerCLSIDChangedEvent = EventManager.RegisterRoutedEvent(
        name: nameof(PreviewHandlerCLSIDChanged),
        routingStrategy: RoutingStrategy.Bubble,
        handlerType: typeof(PreviewHandlerCLSIDChangedEventHandler),
        ownerType: typeof(PreviewHandler));

    /// <summary>
    /// Identifies the routed event raised when <see cref="PreviewHandlerSuccess"/> changes.
    /// </summary>
    public static readonly RoutedEvent PreviewHandlerSuccessChangedEvent = EventManager.RegisterRoutedEvent(
        name: nameof(PreviewHandlerSuccessChanged),
        routingStrategy: RoutingStrategy.Bubble,
        handlerType: typeof(RoutedEventHandler),
        ownerType: typeof(PreviewHandler));

    /// <summary>
    /// Identifies the routed event raised when <see cref="PreviewHandlerLoading"/> changes.
    /// </summary>
    public static readonly RoutedEvent PreviewHandlerLoadingChangedEvent = EventManager.RegisterRoutedEvent(
        name: nameof(PreviewHandlerLoadingChanged),
        routingStrategy: RoutingStrategy.Bubble,
        handlerType: typeof(RoutedEventHandler),
        ownerType: typeof(PreviewHandler));

    /// <summary>
    /// Identifies the <see cref="PreviewHandlerSource"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PreviewHandlerSourceProperty = DependencyProperty.Register(
        name: nameof(PreviewHandlerSource),
        propertyType: typeof(string),
        ownerType: typeof(PreviewHandler),
        typeMetadata: new PropertyMetadata(
            defaultValue: default(string),
            propertyChangedCallback: (s, e) => {
                if (s is PreviewHandler self) {
                    self.OnPreviewHandlerSourceChanged(e);
                }
            }));

    /// <summary>
    /// Identifies the <see cref="PreviewHandlerRefresh"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PreviewHandlerRefreshProperty = DependencyProperty.Register(
        name: nameof(PreviewHandlerRefresh),
        propertyType: typeof(bool),
        ownerType: typeof(PreviewHandler),
        typeMetadata: new PropertyMetadata(
            defaultValue: false,
            propertyChangedCallback: (s, e) => {
                if (s is PreviewHandler self) {
                    self.OnPreviewHandlerRefreshChanged(e);
                }
            }));

    /// <summary>
    /// Identifies the <see cref="PreviewHandlerEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PreviewHandlerEnabledProperty = DependencyProperty.Register(
        name: nameof(PreviewHandlerEnabled),
        propertyType: typeof(bool),
        ownerType: typeof(PreviewHandler),
        typeMetadata: new PropertyMetadata(
            defaultValue: true,
            propertyChangedCallback: (s, e) => {
                if (s is PreviewHandler self) {
                    self.OnPreviewHandlerEnabledChanged(e);
                }
            }));

    /// <summary>
    /// Identifies the <see cref="PreviewHandlerSuccess"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PreviewHandlerSuccessProperty =
        PreviewHandlerSuccessPropertyKey.DependencyProperty;

    /// <summary>
    /// Identifies the <see cref="PreviewHandlerLoading"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PreviewHandlerLoadingProperty =
        PreviewHandlerLoadingPropertyKey.DependencyProperty;


    /// <summary>
    /// Occurs when the control resolves the preview handler class identifier for a file.
    /// Handlers can set <see cref="PreviewHandlerCLSIDChangedEventArgs.Override"/> to select another handler.
    /// </summary>
    public event PreviewHandlerCLSIDChangedEventHandler PreviewHandlerCLSIDChanged {
        add => AddHandler(PreviewHandlerCLSIDChangedEvent, value);
        remove => RemoveHandler(PreviewHandlerCLSIDChangedEvent, value);
    }

    /// <summary>
    /// Occurs when <see cref="PreviewHandlerSuccess"/> changes.
    /// </summary>
    public event RoutedEventHandler PreviewHandlerSuccessChanged {
        add => AddHandler(PreviewHandlerSuccessChangedEvent, value);
        remove => RemoveHandler(PreviewHandlerSuccessChangedEvent, value);
    }

    /// <summary>
    /// Occurs when <see cref="PreviewHandlerLoading"/> changes.
    /// </summary>
    public event RoutedEventHandler PreviewHandlerLoadingChanged {
        add => AddHandler(PreviewHandlerLoadingChangedEvent, value);
        remove => RemoveHandler(PreviewHandlerLoadingChangedEvent, value);
    }

    /// <summary>
    /// Gets or sets the path of the file to preview.
    /// </summary>
    public string PreviewHandlerSource {
        get => GetValue(PreviewHandlerSourceProperty) as string;
        set => SetValue(PreviewHandlerSourceProperty, value);
    }

    /// <summary>
    /// Gets a value indicating whether the current file was successfully previewed.
    /// </summary>
    public bool PreviewHandlerSuccess {
        get => GetValue(PreviewHandlerSuccessProperty) as bool? ?? false;
        private set { SetValue(PreviewHandlerSuccessPropertyKey, value); }
    }

    /// <summary>
    /// Gets a value indicating whether the control is currently loading a preview.
    /// </summary>
    public bool PreviewHandlerLoading {
        get => GetValue(PreviewHandlerLoadingProperty) as bool? ?? false;
        private set { SetValue(PreviewHandlerLoadingPropertyKey, value); }
    }

    /// <summary>
    /// Gets or sets a value that requests a preview refresh when set to <see langword="true"/>.
    /// The control resets the value to <see langword="false"/> after processing the request.
    /// </summary>
    public bool PreviewHandlerRefresh {
        get => GetValue(PreviewHandlerRefreshProperty) as bool? ?? false;
        set => SetValue(PreviewHandlerRefreshProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether file previews are enabled.
    /// Disabling the control unloads the active preview.
    /// </summary>
    public bool PreviewHandlerEnabled {
        get => GetValue(PreviewHandlerEnabledProperty) as bool? ?? true;
        set => SetValue(PreviewHandlerEnabledProperty, value);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PreviewHandler"/> control.
    /// </summary>
    public PreviewHandler() {
        Loaded += This_Loaded;
        Unloaded += This_Unloaded;
        InitializeComponent();
    }
}
