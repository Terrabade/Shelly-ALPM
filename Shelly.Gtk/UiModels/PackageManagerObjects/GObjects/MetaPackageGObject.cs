using GObject;
using Shelly.Gtk.UiModels;

namespace Shelly.Gtk.UiModels.PackageManagerObjects.GObjects;

[Subclass<GObject.Object>]
public partial class MetaPackageGObject
{
    public MetaPackageModel? Package { get; set; }
    public bool IsSelected { get; set; }

    public event EventHandler? OnSelectionToggled;

    public void ToggleSelection()
    {
        IsSelected = !IsSelected;
        OnSelectionToggled?.Invoke(this, EventArgs.Empty);
    }
}
