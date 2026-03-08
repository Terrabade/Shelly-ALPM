using Gtk;

namespace Shelly.Gtk.Helpers;

public static class ColumnViewHelper
{
    public static void AlignColumnHeader(ColumnView columnView, int columnIndex, Align alignment)
    {
        columnView.OnRealize += (s, e) => { ApplyAlignment(columnView, columnIndex, alignment); };
        
        if (columnView.GetRealized())
        {
            ApplyAlignment(columnView, columnIndex, alignment);
        }
    }

    private static void ApplyAlignment(ColumnView columnView, int columnIndex, Align alignment)
    {
        var header = FindHeaderContainer(columnView);
        if (header == null) return;

        var button = GetNthChild(header, columnIndex);
        var content = button?.GetFirstChild();
        content?.SetHalign(alignment);
    }

    private static Widget? FindHeaderContainer(Widget root)
    {
        var queue = new Queue<Widget>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            
            var child = current.GetFirstChild();
            while (child != null)
            {
                if (child.HasCssClass("column-header"))
                {
                    return current;
                }
                queue.Enqueue(child);
                child = child.GetNextSibling();
            }
        }
        
        var firstChild = root.GetFirstChild();
        return firstChild ?? null;
    }

    private static Widget? GetNthChild(Widget widget, int index)
    {
        var child = widget.GetFirstChild();
        for (var i = 0; child != null && i < index; i++)
        {
            child = child.GetNextSibling();
        }
        return child;
    }
}
