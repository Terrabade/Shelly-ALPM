using Shelly.Gtk.UiModels;

namespace Shelly.Gtk.Services;

public interface IGenericQuestionService
{
    event EventHandler<GenericQuestionEventArgs>? Question;
    event EventHandler<PackageBuildEventArgs>? PackageBuildRequested;
    void RaiseQuestion(GenericQuestionEventArgs args);
    void RaisePackageBuild(PackageBuildEventArgs args);
}
