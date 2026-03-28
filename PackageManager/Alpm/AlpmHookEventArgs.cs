using System;

namespace PackageManager.Alpm;

public class AlpmHookEventArgs(string description, ulong position, ulong total) : EventArgs
{
    public string Description { get; } = description;
    public ulong Position { get; } = position;
    public ulong Total { get; } = total;
}
