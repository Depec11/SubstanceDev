using Substance.Logging;

namespace Substance.Graphics;

public class RenderingServer : IDisposable
{
    public static RenderEngine Current { get; private set; } = new RenderEngine();

    private bool _disposed = false;

    internal RenderingServer() {}

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

        Current = api switch
        {
            GraphicApi.None => new RenderEngine(),
            GraphicApi.OpenGL => 
#if ANDROID
            new RenderEngineGLES(),
#else
            new RenderEngineGL(),
#endif
            _ => throw new Exception($"未支持的渲染API: {api}"),
        };

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
