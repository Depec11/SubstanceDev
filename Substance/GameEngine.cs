using Substance.Graphics;

namespace Substance;

public class GameEngine : IDisposable
{
    private RenderingServer _renderEngineManager = new();
    private bool _disposed = false;

    internal GameEngine() {}

    internal void Update(double deltaTime) {}

    internal void Render(double deltaTime)
    {
        RenderingServer.Current.BeforeDraw();
#if DEBUG
        RenderingServer.Current.DrawTestRect();
#endif
        RenderingServer.Current.AfterDraw();
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