using System.Runtime.CompilerServices;

namespace Substance;

public class GameLoop : IDisposable
{
    private bool _disposed = false;

    public GameLoop()
    {
        Application.GameEngine.Initialized += OnGameEngineInitialized;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void OnGameEngineInitialized()
    {
        OnInitializedOverride();
    }

    protected virtual void OnInitializedOverride() {}

    protected virtual void OnDisposedOverride() {}

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        OnDisposedOverride();

        Application.GameEngine.Initialized -= OnGameEngineInitialized;

        GC.SuppressFinalize(this);
    }
}