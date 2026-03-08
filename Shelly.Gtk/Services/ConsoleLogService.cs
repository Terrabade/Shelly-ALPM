using System.Text;

namespace Shelly.Gtk.Services;

public class ConsoleLogService(ILockoutService lockoutService, bool isError) : TextWriter
{
    private TextWriter? _original;
    private bool _isStarted;

    public void Start()
    {
        if (_isStarted) return;
        _original = isError ? Console.Error : Console.Out;
        if (isError) Console.SetError(this);
        else Console.SetOut(this);
        _isStarted = true;
    }

    public void Stop()
    {
        if (!_isStarted) return;
        if (_original != null)
        {
            if (isError) Console.SetError(_original);
            else Console.SetOut(_original);
        }
        _isStarted = false;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Stop();
        }
        base.Dispose(disposing);
    }

    public override void WriteLine(string? value)
    {
        _original?.WriteLine(value);
        if (!string.IsNullOrEmpty(value))
        {
            lockoutService.ParseLog(value);
        }
    }

    public override void WriteLine()
    {
        _original?.WriteLine();
    }

    public override void Write(string? value)
    {
        _original?.Write(value);
    }

    public override void Write(char[] buffer, int index, int count)
    {
        _original?.Write(buffer, index, count);
    }

    public override void Write(char value)
    {
        _original?.Write(value);
    }

    public override Encoding Encoding => _original?.Encoding ?? Encoding.UTF8;

    public override void Flush()
    {
        _original?.Flush();
    }
}
