using PackageManager.Utilities;

namespace PackageManager.Tests.UtilitiesTests;

public class PkgbuildParserTests
{
    [Test]
    public void ParseContent_ResolvesSimpleVariableSubstitution_InDepends()
    {
        var pkgbuild = """
                       pkgname=simple-web-server
                       pkgver=1.2.17
                       pkgrel=1
                       _electronversion=38
                       depends=("electron${_electronversion}")
                       makedepends=('curl' 'gendesk' 'git' 'npm' 'nvm')
                       """;

        var result = PkgbuildParser.ParseContent(pkgbuild);

        Assert.That(result.Depends, Has.Count.EqualTo(1));
        Assert.That(result.Depends[0], Is.EqualTo("electron38"));
    }

    [Test]
    public void ParseContent_ResolvesVariableWithoutBraces_InDepends()
    {
        var pkgbuild = """
                       _electronversion=38
                       depends=("electron$_electronversion")
                       """;

        var result = PkgbuildParser.ParseContent(pkgbuild);

        Assert.That(result.Depends, Has.Count.EqualTo(1));
        Assert.That(result.Depends[0], Is.EqualTo("electron38"));
    }

    [Test]
    public void ParseContent_ResolvesMultipleVariables_InSingleDep()
    {
        var pkgbuild = """
                       _pkgname=myapp
                       _ver=2
                       depends=("${_pkgname}-libs${_ver}")
                       """;

        var result = PkgbuildParser.ParseContent(pkgbuild);

        Assert.That(result.Depends, Has.Count.EqualTo(1));
        Assert.That(result.Depends[0], Is.EqualTo("myapp-libs2"));
    }

    [Test]
    public void ParseContent_KeepsLiteralWhenVariableNotFound()
    {
        var pkgbuild = """
                       depends=("electron${_undefined}")
                       """;

        var result = PkgbuildParser.ParseContent(pkgbuild);

        Assert.That(result.Depends, Has.Count.EqualTo(1));
        Assert.That(result.Depends[0], Is.EqualTo("electron${_undefined}"));
    }

    [Test]
    public void ParseContent_LeavesPlainDepsUnchanged()
    {
        var pkgbuild = """
                       depends=('pacman' 'gtk4' 'glib2')
                       """;

        var result = PkgbuildParser.ParseContent(pkgbuild);

        Assert.That(result.Depends, Has.Count.EqualTo(3));
        Assert.That(result.Depends[0], Is.EqualTo("pacman"));
        Assert.That(result.Depends[1], Is.EqualTo("gtk4"));
        Assert.That(result.Depends[2], Is.EqualTo("glib2"));
    }

    [Test]
    public void ParseContent_ResolvesArrayExpansion()
    {
        var pkgbuild = """
                       _common_deps=('pacman' 'git')
                       depends=("${_common_deps[@]}" 'bash')
                       """;

        var result = PkgbuildParser.ParseContent(pkgbuild);

        Assert.That(result.Depends, Has.Count.EqualTo(3));
        Assert.That(result.Depends, Does.Contain("pacman"));
        Assert.That(result.Depends, Does.Contain("git"));
        Assert.That(result.Depends, Does.Contain("bash"));
    }
}
