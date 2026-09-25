using Brows.Threading;
using Brows.Win32.PlatformInvoke;
using Domore.Logs;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Brows.Win32;

internal sealed class PreviewWorker {
    private static long ID;
    private static readonly ILog Log = Logging.For(typeof(PreviewWorker));

    private readonly Lazy<STAThreadPool> LazyThreadPool = new(() =>
        new(nameof(PreviewWorker) + "-" + Interlocked.Increment(ref ID)) {
            WorkerCountMax = 1,
            WorkerCountMin = 0
        });

    private STAThreadPool ThreadPool => field ??= LazyThreadPool.Value;

    private PreviewWorkerCLSID CLSID => field ??= new PreviewWorkerCLSID(ThreadPool);

    private volatile object State;
    private volatile PreviewHandlerWrapper Handler;
    private readonly
#if NET9_0_OR_GREATER
        Lock
#else
        object
#endif
        Locker = new();

    private void Unload() {
        Try(nameof(Handler.Unload), () => Handler?.Unload(), swallow: true);
        Try(nameof(Handler.Dispose), () => Handler?.Dispose(), swallow: true);
        Handler = null;
    }

    private Task Unload(object state, CancellationToken cancellationToken) {
        if (state != State) {
            return Task.CompletedTask;
        }
        var unloadState = State = new object();
        return ThreadPool.Work(nameof(Unload), cancellationToken: cancellationToken, work: () => {
            if (unloadState != State) {
                return;
            }
            lock (Locker) {
                if (unloadState != State) {
                    return;
                }
                Unload();
                ThreadPool.Empty();
            }
        });
    }

    private async Task<bool> UnloadAndReturnFalse(object state) {
        await Unload(state, CancellationToken.None);
        return false;
    }

    private static void Try(string name, Action action, bool swallow = false) {
        if (action is null) {
            throw new ArgumentNullException(nameof(action));
        }
        try {
            action();
        }
        catch (Exception ex) {
            if (swallow) {
                if (Log.Info()) {
                    Log.Info(name, ex);
                }
            }
            else {
                if (Log.Debug()) {
                    Log.Debug(name, ex);
                }
                throw;
            }
        }
    }

    private async Task<Guid> ChangeCLSID(string extension, CancellationToken cancellationToken) {
        var clsid = await CLSID.For(extension, cancellationToken);
        var change = new PreviewWorkerCLSIDChangedEventArgs(extension, clsid);
        CLSIDChanged?.Invoke(this, change);
        return change.Override.HasValue
            ? change.Override.Value
            : change.CLSID;
    }

    public event PreviewWorkerCLSIDChangedEventHandler CLSIDChanged;

    public Task Change(RECT? rect, CancellationToken cancellationToken) {
        if (rect == null) {
            return Task.CompletedTask;
        }
        var handler = Handler;
        if (handler is null) {
            return Task.CompletedTask;
        }
        return ThreadPool.Work(nameof(handler.SetRect), cancellationToken: cancellationToken, work: () => {
            if (!ReferenceEquals(handler, Handler)) {
                return;
            }
            lock (Locker) {
                if (!ReferenceEquals(handler, Handler)) {
                    return;
                }
                Try(nameof(handler.SetRect), () => handler.SetRect(rect.Value));
            }
        });
    }

    public Task Unload(CancellationToken cancellationToken) {
        var state = State = new object();
        return Unload(state, cancellationToken);
    }

    public async Task<bool> Start(string file, nint? hwnd, RECT? rect, CancellationToken cancellationToken) {
        var state = State = new object();
        if (string.IsNullOrEmpty(file)) {
            return await UnloadAndReturnFalse(state);
        }
        if (rect == null) {
            return await UnloadAndReturnFalse(state);
        }
        if (hwnd == null || hwnd == 0) {
            return await UnloadAndReturnFalse(state);
        }
        var ext = Path.GetExtension(file);
        var clsid = default(Guid);
        try {
            clsid = await ChangeCLSID(ext, cancellationToken);
        }
        catch {
            await Unload(state, CancellationToken.None);
            throw;
        }
        if (!ReferenceEquals(state, State)) {
            return false;
        }
        if (clsid == Guid.Empty) {
            return await UnloadAndReturnFalse(state);
        }
        return await ThreadPool.Work(nameof(Handler.DoPreview), cancellationToken: cancellationToken, work: () => {
            if (!ReferenceEquals(state, State)) {
                return false;
            }
            lock (Locker) {
                if (!ReferenceEquals(state, State)) {
                    return false;
                }
                Unload();
                var handler = Handler = new PreviewHandlerWrapper(clsid);
                try {
                    Try(nameof(handler.Initialize), () => handler.Initialize(file));
                    cancellationToken.ThrowIfCancellationRequested();

                    Try(nameof(handler.SetWindow), () => handler.SetWindow(hwnd.Value, rect.Value));
                    cancellationToken.ThrowIfCancellationRequested();

                    Try(nameof(handler.DoPreview), () => handler.DoPreview());
                    cancellationToken.ThrowIfCancellationRequested();

                    Try(nameof(handler.SetRect), () => handler.SetRect(rect.Value));
                    cancellationToken.ThrowIfCancellationRequested();

                    return true;
                }
                catch {
                    Unload();
                    throw;
                }
            }
        });
    }
}
