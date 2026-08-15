namespace Substance.Core.Events;

public abstract class EventArgs : IDisposable
{
    public EventType Type { get; }

    private bool _disposed = false;

    protected EventArgs(EventType type)
    {
        Type = type;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        GC.SuppressFinalize(this);
    }
}