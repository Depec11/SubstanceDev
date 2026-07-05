namespace Substance;

public class GameEngine : IDisposable
{
    private bool _disposed;

    internal GameEngine() {}

    internal void Update(double deltaTime) {}

    internal void Render(double deltaTime) {}

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