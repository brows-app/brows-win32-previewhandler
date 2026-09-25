using Brows.Win32.InteropServices;
using Brows.Windows;
using System.Threading;
using PreviewHandlerControl = Brows.Windows.Controls.PreviewHandler;

namespace Brows.Win32.PreviewHandler.Windows.Tests;

[TestFixture]
[Apartment(ApartmentState.STA)]
public sealed class PreviewHandlerTests {
    [Test]
    public void Constructor_InitializesControlDefaults() {
        var control = new PreviewHandlerControl();

        Assert.That(control.PreviewHandlerSource, Is.Null);
        Assert.That(control.PreviewHandlerEnabled, Is.True);
        Assert.That(control.PreviewHandlerRefresh, Is.False);
        Assert.That(control.PreviewHandlerSuccess, Is.False);
        Assert.That(control.PreviewHandlerLoading, Is.False);
    }

    [Test]
    public void RefreshRequest_ResetsToFalse() {
        var control = new PreviewHandlerControl();

        control.PreviewHandlerRefresh = true;

        Assert.That(control.PreviewHandlerRefresh, Is.False);
    }

    [Test]
    public void Enabled_CanBeTurnedOffAndBackOn() {
        var control = new PreviewHandlerControl();

        control.PreviewHandlerEnabled = false;
        Assert.That(control.PreviewHandlerEnabled, Is.False);

        control.PreviewHandlerEnabled = true;
        Assert.That(control.PreviewHandlerEnabled, Is.True);
    }

    [Test]
    public void CLSIDChangedEventArgs_PreserveResolvedValuesAndAllowOverride() {
        var clsid = new Guid("8895B1C6-B41F-4C1C-A562-0D564250836F");
        var replacement = new Guid("196BF9A5-B346-4EF0-AA1E-5DCDB76768B1");
        var routedEvent = PreviewHandlerControl.PreviewHandlerCLSIDChangedEvent;
        var args = new PreviewHandlerCLSIDChangedEventArgs(".txt", clsid, routedEvent);

        Assert.That(args.Extension, Is.EqualTo(".txt"));
        Assert.That(args.CLSID, Is.EqualTo(clsid));
        Assert.That(args.RoutedEvent, Is.EqualTo(routedEvent));
        Assert.That(args.Override, Is.Null);

        args.Override = replacement;

        Assert.That(args.Override, Is.EqualTo(replacement));
    }

    [Test]
    public void Wrapper_PreservesClassIdentifierWithoutActivatingComObject() {
        var clsid = Guid.NewGuid();
        using var wrapper = new PreviewHandlerWrapper(clsid);

        Assert.That(wrapper.Clsid, Is.EqualTo(clsid));
    }

    [Test]
    public void WorkerClassIdentifierEventArgs_PreserveValuesAndAllowOverride() {
        var clsid = Guid.NewGuid();
        var replacement = Guid.NewGuid();
        var args = new PreviewWorkerCLSIDChangedEventArgs(".txt", clsid);

        Assert.That(args.Extension, Is.EqualTo(".txt"));
        Assert.That(args.CLSID, Is.EqualTo(clsid));
        Assert.That(args.Override, Is.Null);

        args.Override = replacement;

        Assert.That(args.Override, Is.EqualTo(replacement));
    }
}
