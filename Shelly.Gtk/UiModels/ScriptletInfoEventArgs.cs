namespace Shelly.Gtk.UiModels;

public class ScriptletInfoEventArgs(string line) : EventArgs
{
    public string Line { get; } = line;
}
