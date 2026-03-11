using GObject;

namespace Shelly.Gtk.UiModels.PackageManagerObjects.GObjects;

[Subclass<GObject.Object>]
public partial class PackageDetailGObject
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
