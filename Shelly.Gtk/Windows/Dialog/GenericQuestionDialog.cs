using Gtk;
using Shelly.Gtk.UiModels;

namespace Shelly.Gtk.Windows.Dialog;

public static class GenericQuestionDialog
{
    public static void ShowGenericQuestionDialog(Overlay parentOverlay, GenericQuestionEventArgs e)
    {
        var box = Box.New(Orientation.Vertical, 12);
        box.SetHalign(Align.Center);
        box.SetValign(Align.Center);
        box.SetSizeRequest(400, -1);
        box.SetMarginTop(20);
        box.SetMarginBottom(20);
        box.SetMarginStart(20);
        box.SetMarginEnd(20);
        box.AddCssClass("dialog-overlay");

        var titleLabel = Label.New(e.Title);
        titleLabel.AddCssClass("title-4");
        box.Append(titleLabel);

        var messageLabel = Label.New(e.Message);
        messageLabel.SetWrap(true);

        var scrolledWindow = new ScrolledWindow();
        scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Automatic);
        scrolledWindow.SetMaxContentHeight(300);
        scrolledWindow.SetPropagateNaturalHeight(true);
        scrolledWindow.SetChild(messageLabel);
        box.Append(scrolledWindow);

        var buttonBox = Box.New(Orientation.Horizontal, 8);
        buttonBox.SetHalign(Align.End);

        var noButton = Button.NewWithLabel("No");
        var yesButton = Button.NewWithLabel("Yes");
        yesButton.AddCssClass("suggested-action");

        noButton.OnClicked += (s, args) =>
        {
            e.SetResponse(false);
            parentOverlay.RemoveOverlay(box);
        };

        yesButton.OnClicked += (s, args) =>
        {
            e.SetResponse(true);
            parentOverlay.RemoveOverlay(box);
        };

        buttonBox.Append(yesButton);
        buttonBox.Append(noButton);
        box.Append(buttonBox);

        parentOverlay.AddOverlay(box);
    }
}
