using Brows.Win32.InteropServices;
using Brows.Win32.PlatformInvoke;
using System.Runtime.InteropServices;

namespace Brows.Win32.PreviewHandler.Windows.Tests;

[TestFixture]
public sealed class HRESULTExtensionTests {
    [TestCase(0u)]
    [TestCase(1u)]
    [TestCase(0x000401A0u)]
    public void ThrowOnError_DoesNotThrowForSuccessfulResults(uint value) {
        Assert.DoesNotThrow(() => ((HRESULT)value).ThrowOnError());
    }

    [Test]
    public void ThrowOnError_DoesNotThrowForUserCancellation() {
        var hr = (HRESULT)0x80270000u;

        Assert.DoesNotThrow(() => hr.ThrowOnError());
    }

    [Test]
    public void ThrowOnError_ThrowsComExceptionForFailure() {
        var exception = Assert.Throws<COMException>(() => HRESULT.E_FAIL.ThrowOnError());

        Assert.That(exception.ErrorCode, Is.EqualTo(unchecked((int)HRESULT.E_FAIL)));
    }
}

[TestFixture]
public sealed class LOGFONTWTests {
    [Test]
    public void FaceName_IsEmptyByDefault() {
        var font = new LOGFONTW();

        Assert.That(font.lfFaceName, Is.Empty);
    }

    [TestCase("")]
    [TestCase("Segoe UI")]
    [TestCase("1234567890123456789012345678901")]
    public void FaceName_RoundTripsStringsUpTo31Characters(string value) {
        var font = new LOGFONTW {
            lfFaceName = value
        };

        Assert.That(font.lfFaceName, Is.EqualTo(value));
    }

    [Test]
    public void FaceName_ClearsCharactersAfterShorterReplacement() {
        var font = new LOGFONTW {
            lfFaceName = "Long font name"
        };

        font.lfFaceName = "UI";

        Assert.That(font.lfFaceName, Is.EqualTo("UI"));
    }

    [Test]
    public void FaceName_RejectsMoreThan31Characters() {
        var font = new LOGFONTW();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            font.lfFaceName = "12345678901234567890123456789012");
    }

    [Test]
    public void FaceName_RejectsNull() {
        var font = new LOGFONTW();

        Assert.Throws<ArgumentNullException>(() => font.lfFaceName = null);
    }
}

[TestFixture]
public sealed class IIDTests {
    [Test]
    public void ManagedIdentifiers_MatchTheirStringIdentifiers() {
        Assert.That(IID.Managed.IShellItem, Is.EqualTo(new Guid(IID.IShellItem)));
        Assert.That(IID.Managed.IPreviewHandler, Is.EqualTo(new Guid(IID.IPreviewHandler)));
    }
}
