using Gtk;
using System.Text;
using System.Text.RegularExpressions;

namespace Shelly.Gtk.Windows.Dialog;

public static partial class ReleaseNotesDialog
{
    public static void ShowReleaseNotesDialog(Overlay parentOverlay, string markdown)
    {
        var box = Box.New(Orientation.Vertical, 12);
        box.SetHalign(Align.Center);
        box.SetValign(Align.Center);
        box.SetSizeRequest(600, 500);
        box.SetMarginTop(40);
        box.SetMarginBottom(40);
        box.SetMarginStart(40);
        box.SetMarginEnd(40);
        box.AddCssClass("dialog-overlay");

        var titleLabel = Label.New("What's New");
        titleLabel.AddCssClass("title-2");
        box.Append(titleLabel);

        var contentBox = Box.New(Orientation.Vertical, 6);
        contentBox.SetMarginTop(10);
        contentBox.SetMarginBottom(10);
        contentBox.SetMarginStart(10);
        contentBox.SetMarginEnd(10);

        ParseMarkdown(contentBox, markdown);

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
        closeButton.OnClicked += (s, args) =>
        {
            parentOverlay.RemoveOverlay(box);
        };

        buttonBox.Append(closeButton);
        box.Append(buttonBox);

        parentOverlay.AddOverlay(box);
    }

    private static void ParseMarkdown(Box container, string markdown)
    {
        var lines = markdown.Split('\n');
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                continue;
            }

            if (trimmedLine.StartsWith("## "))
            {
                var label = Label.New(string.Empty);
                label.SetMarkup($"<span size='large' weight='bold'>{GLib.Markup.EscapeText(trimmedLine[3..])}</span>");
                label.SetHalign(Align.Start);
                label.SetMarginTop(10);
                container.Append(label);
            }
            else if (trimmedLine.StartsWith("* "))
            {
                var label = Label.New(string.Empty);
                var content = ProcessInlineMarkdown(trimmedLine[2..]);
                label.SetMarkup($"• {content}");
                label.SetHalign(Align.Start);
                label.SetXalign(0);
                label.SetWrap(true);
                label.SetMarginStart(12);
                container.Append(label);
            }
            else
            {
                var label = Label.New(string.Empty);
                var content = ProcessInlineMarkdown(trimmedLine);
                label.SetMarkup(content);
                label.SetHalign(Align.Start);
                label.SetXalign(0);
                label.SetWrap(true);
                container.Append(label);
            }
        }
    }

    private static string ProcessInlineMarkdown(string text)
    {
        var escaped = GLib.Markup.EscapeText(text);

        var result = BoldRegex().Replace(escaped, "<b>$1</b>");

        result = UrlRegex().Replace(result, "<a href='$1'>$1</a>");
        
        result = MentionRegex().Replace(result, "<b>$1</b>");

        return result;
    }

    [GeneratedRegex(@"\*\*(.*?)\*\*")]
    private static partial Regex BoldRegex();
    [GeneratedRegex(@"(https?://[^\s]+)")]
    private static partial Regex UrlRegex();
    [GeneratedRegex(@"(@[a-zA-Z0-9_-]+)")]
    private static partial Regex MentionRegex();
}
