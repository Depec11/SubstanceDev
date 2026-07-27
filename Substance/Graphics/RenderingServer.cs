using Substance.Logging;

namespace Substance.Graphics;

public class RenderingServer : IDisposable
{
    public static RenderEngine Current { get; private set; } = new RenderEngine();

    private Func<GraphicApi, RenderEngine>? _createRenderEngine = null;
    private bool _disposed = false;
    
    internal RenderingServer(Func<GraphicApi, RenderEngine> createRenderEngine)
    {
        _createRenderEngine = createRenderEngine;
    }

    ~RenderingServer()
    {
        Dispose();
    }

    internal void MakeRenderEngine(GraphicApi api)
    {
        if (api == Current.Api)
        {
            return;
        }

        Current.Dispose();

        Current = _createRenderEngine is null ? new RenderEngine() : _createRenderEngine(api);

        Log.Info($"[{nameof(RenderingServer)}] 渲染引擎已切换为 {api}");
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        
        Current.Dispose();
        
        GC.SuppressFinalize(this);
    }
}
