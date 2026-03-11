using GObject;

namespace Shelly.Gtk.UiModels.PackageManagerObjects.GObjects;

[Subclass<GObject.Object>]
public partial class RepositoryGroupGObject
{
    public string RepositoryName { get; set; } = string.Empty;
    public List<AlpmPackageGObject> Children { get; set; } = [];
}
