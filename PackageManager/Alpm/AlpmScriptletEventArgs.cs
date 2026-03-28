using System;

namespace PackageManager.Alpm;

public class AlpmScriptletEventArgs(string line) : EventArgs
{
    public string Line { get; } = line;
}
