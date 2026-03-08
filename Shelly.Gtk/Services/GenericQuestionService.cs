using Shelly.Gtk.UiModels;

namespace Shelly.Gtk.Services;

public class GenericQuestionService : IGenericQuestionService
{
    public event EventHandler<GenericQuestionEventArgs>? Question;
    public event EventHandler<PackageBuildEventArgs>? PackageBuildRequested;

    public void RaiseQuestion(GenericQuestionEventArgs args)
    {
        Question?.Invoke(this, args);
    }

    public void RaisePackageBuild(PackageBuildEventArgs args)
    {
        PackageBuildRequested?.Invoke(this, args);
    }
}
