using Gtk;
using Shelly.Gtk.UiModels;

namespace Shelly.Gtk.Windows.Dialog;

public static class PackageBuildDialog
{
    public static void ShowPackageBuildDialog(Overlay parentOverlay, PackageBuildEventArgs e)
    {
        var box = Box.New(Orientation.Vertical, 12);
        box.SetHalign(Align.Center);
        box.SetValign(Align.Center);
        box.SetSizeRequest(600, 500);
        box.SetMarginTop(20);
        box.SetMarginBottom(20);
        box.SetMarginStart(20);
        box.SetMarginEnd(20);
        box.AddCssClass("dialog-overlay");

        var titleLabel = Label.New(e.Title);
        titleLabel.AddCssClass("title-4");
        box.Append(titleLabel);

        var pkgBuildLabel = Label.New(e.PkgBuild);
        pkgBuildLabel.SetWrap(false);
        pkgBuildLabel.SetXalign(0);
        pkgBuildLabel.AddCssClass("monospace");

        var scrolledWindow = new ScrolledWindow();
        scrolledWindow.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
        scrolledWindow.SetVexpand(true);
        scrolledWindow.SetChild(pkgBuildLabel);
        box.Append(scrolledWindow);

        var buttonBox = Box.New(Orientation.Horizontal, 8);
        buttonBox.SetHalign(Align.End);

        var cancelButton = Button.NewWithLabel("Cancel");
        var confirmButton = Button.NewWithLabel("Confirm");
        confirmButton.AddCssClass("suggested-action");

        cancelButton.OnClicked += (s, args) =>
        {
            e.SetResponse(false);
            parentOverlay.RemoveOverlay(box);
        };

        confirmButton.OnClicked += (s, args) =>
        {
            e.SetResponse(true);
            parentOverlay.RemoveOverlay(box);
        };

        buttonBox.Append(confirmButton);
        buttonBox.Append(cancelButton);
        box.Append(buttonBox);

        parentOverlay.AddOverlay(box);
    }
}
