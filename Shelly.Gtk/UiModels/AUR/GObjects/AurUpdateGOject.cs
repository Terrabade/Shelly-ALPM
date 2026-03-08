using GObject;
using Shelly.Gtk.UiModels.PackageManagerObjects;

namespace Shelly.Gtk.UiModels.AUR.GObjects;

[Subclass<GObject.Object>]
public partial class AurUpdateGObject
{
    public AurUpdateDto? Package { get; set; }
    public bool IsSelected { get; set; }

    public event EventHandler? OnSelectionToggled;

    public void ToggleSelection()
    {
        IsSelected = !IsSelected;
        OnSelectionToggled?.Invoke(this, EventArgs.Empty);
    }
}