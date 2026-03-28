namespace Shelly.Gtk.UiModels;

public class HookInfoEventArgs(string line) : EventArgs
{
    public string Line { get; } = line;
}
