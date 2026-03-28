using Shelly.Gtk.UiModels;

namespace Shelly.Gtk.Services;

public class AlpmEventService : IAlpmEventService
{
    public event EventHandler<QuestionEventArgs>? Question;
    public event EventHandler<PackageOperationEventArgs>? PackageOperation;
    public event EventHandler<ScriptletInfoEventArgs>? ScriptletInfo;
    public event EventHandler<HookInfoEventArgs>? HookInfo;

    public void RaiseQuestion(QuestionEventArgs args)
    {
        Question?.Invoke(this, args);
    }

    public void RaisePackageOperation(PackageOperationEventArgs args)
    {
        PackageOperation?.Invoke(this, args);
    }

    public void RaiseScriptletInfo(ScriptletInfoEventArgs args)
    {
        ScriptletInfo?.Invoke(this, args);
    }

    public void RaiseHookInfo(HookInfoEventArgs args)
    {
        HookInfo?.Invoke(this, args);
    }
}