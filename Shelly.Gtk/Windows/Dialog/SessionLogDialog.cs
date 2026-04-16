using Gtk;
using System.Text;
using System.Text.RegularExpressions;
using Shelly.Gtk.UiModels;

namespace Shelly.Gtk.Windows.Dialog;

public static class SessionLogDialog
{
    private static void CloseDialog(Overlay parentOverlay, Widget dialogBox)
    {

        try
        {
            parentOverlay.RemoveOverlay(dialogBox);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao remover overlay: {ex.Message}");
        }
        finally
        {
            dialogBox.Dispose();
        }
    }
    
    public static Widget ShowSessionLogDialog(Overlay parentOverlay, OperationLogEntry entry)
    {
        var box = Box.New(Orientation.Vertical, 12);
        box.SetHalign(Align.Center);
        box.SetValign(Align.Center);
        box.SetSizeRequest(700, 550);
        box.SetMarginTop(40);
        box.SetMarginBottom(40);
        box.SetMarginStart(40);
        box.SetMarginEnd(40);
        box.AddCssClass("dialog-overlay");

        var headerBox = Box.New(Orientation.Horizontal, 10);
        headerBox.SetMarginTop(4);
        headerBox.SetMarginStart(4);
        headerBox.SetMarginEnd(4);

        var titleLabel = Label.New("Session Log");
        titleLabel.AddCssClass("title-2");
        titleLabel.SetHalign(Align.Start);
        titleLabel.SetHexpand(true);
        headerBox.Append(titleLabel);

        headerBox.Append(BuildSessionMetaBadge(entry));
        box.Append(headerBox);

        var contentBox = Box.New(Orientation.Vertical, 12);
        contentBox.SetMarginTop(10);
        contentBox.SetMarginBottom(10);
        contentBox.SetMarginStart(10);
        contentBox.SetMarginEnd(10);
        contentBox.Append(BuildSessionLogCard(entry, entry.RawLines));

        var scrolledWindow = new ScrolledWindow();
        scrolledWindow.SetPolicy(PolicyType.Never, PolicyType.Automatic);
        scrolledWindow.SetVexpand(true);
        scrolledWindow.SetChild(contentBox);
        box.Append(scrolledWindow);

        var buttonBox = Box.New(Orientation.Horizontal, 8);
        buttonBox.SetHalign(Align.End);
        buttonBox.SetMarginTop(10);

        var closeButton = Button.NewWithLabel("Close");
        closeButton.AddCssClass("suggested-action");
        
        closeButton.OnClicked += (_, _) => CloseDialog(parentOverlay, box);

        buttonBox.Append(closeButton);
        box.Append(buttonBox);
        
        parentOverlay.AddOverlay(box); 

        return box;
    }

    private static Widget BuildSessionMetaBadge(OperationLogEntry entry)
    {
        var meta = Box.New(Orientation.Horizontal, 8);
        meta.SetValign(Align.Center);

        var userIcon = Image.NewFromIconName(
            entry.IsSudo ? "system-lock-screen-symbolic" : "avatar-default-symbolic");
        userIcon.SetPixelSize(14);
        meta.Append(userIcon);

        var userLabel = Label.New(entry.User);
        userLabel.AddCssClass("dim-label");
        userLabel.AddCssClass("caption");
        meta.Append(userLabel);

        var dot = Label.New("·");
        dot.AddCssClass("dim-label");
        meta.Append(dot);

        var statusLabel = Label.New(
            entry.ExitCode.HasValue ? $"Exit {entry.ExitCode}" : "In progress");
        statusLabel.AddCssClass("caption");
        statusLabel.AddCssClass(
            !entry.ExitCode.HasValue ? "dim-label" :
            entry.ExitCode == 0     ? "success"   : "error");
        meta.Append(statusLabel);

        return meta;
    }

    private static Box BuildSessionLogCard(OperationLogEntry entry, List<string> rawLines)
    {
        var card = Box.New(Orientation.Vertical, 8);
        card.AddCssClass("card");
        card.SetMarginBottom(6);
        card.SetMarginStart(2);
        card.SetMarginEnd(2);
        card.SetMarginTop(2);

        var header = Box.New(Orientation.Horizontal, 8);
        header.SetMarginTop(8);
        header.SetMarginBottom(4);
        header.SetMarginStart(8);
        header.SetMarginEnd(8);

        var cmdLabel = Label.New(entry.Command ?? "");
        cmdLabel.AddCssClass("heading");
        cmdLabel.SetHalign(Align.Start);
        cmdLabel.SetXalign(0);
        cmdLabel.SetHexpand(true);
        cmdLabel.SetEllipsize(Pango.EllipsizeMode.End);

        var dateLabel = Label.New(entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
        dateLabel.AddCssClass("dim-label");
        dateLabel.SetHalign(Align.End);
        dateLabel.SetValign(Align.Center);

        header.Append(cmdLabel);
        header.Append(dateLabel);
        card.Append(header);

        var textView = new TextView();
        textView.Editable = false;
        textView.Monospace = true;
        textView.WrapMode = WrapMode.WordChar;
        textView.SetMarginBottom(10);
        textView.SetMarginStart(8);
        textView.SetMarginEnd(8);

        var buffer = textView.Buffer;

        var fullLogText = string.Join("\n", rawLines);
        buffer.Text = fullLogText;  

        card.Append(textView);

        return card;
    }

    
}