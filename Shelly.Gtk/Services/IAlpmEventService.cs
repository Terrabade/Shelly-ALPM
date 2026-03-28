using Shelly.Gtk.UiModels;

namespace Shelly.Gtk.Services;

public interface IAlpmEventService
{
    event EventHandler<QuestionEventArgs>? Question;
    event EventHandler<PackageOperationEventArgs>? PackageOperation;
    event EventHandler<ScriptletInfoEventArgs>? ScriptletInfo;
    event EventHandler<HookInfoEventArgs>? HookInfo;

    /// <summary>
    /// Raises a Question event. Called by PrivilegedOperationService when parsing CLI stderr.
    /// </summary>
    void RaiseQuestion(QuestionEventArgs args);

    /// <summary>
    /// Raises a PackageOperation event. Called by PrivilegedOperationService when parsing CLI stderr.
    /// </summary>
    void RaisePackageOperation(PackageOperationEventArgs args);

    /// <summary>
    /// Raises a ScriptletInfo event. Called by PrivilegedOperationService when parsing CLI stderr.
    /// </summary>
    void RaiseScriptletInfo(ScriptletInfoEventArgs args);

    /// <summary>
    /// Raises a HookInfo event. Called by PrivilegedOperationService when parsing CLI stderr.
    /// </summary>
    void RaiseHookInfo(HookInfoEventArgs args);
}
