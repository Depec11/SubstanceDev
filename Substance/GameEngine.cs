using Substance.Graphics;

namespace Substance;

public class GameEngine : IDisposable
{
    private RenderEngineManager _renderEngineManager = new();
    private bool _disposed = false;

    internal GameEngine() {}

    internal void Update(double deltaTime) {}

    internal void Render(double deltaTime)
    {
        _renderEngineManager.Current.BeginDraw();
        _renderEngineManager.Current.EndDraw();
    }

    public void MakeRenderEngine(GraphicApi api)
    {
        _renderEngineManager.MakeRenderEngine(api);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _renderEngineManager.Dispose();
    
        GC.SuppressFinalize(this);
    }
}