using Substance.Maths;

namespace Substance.Graphics;

public class RenderEngine : IDisposable
{
    public GraphicApi Api { get; }

    protected readonly IntPtr _windowPtr;
    
    private bool disposed = false;

    internal RenderEngine(GraphicApi api = GraphicApi.None)
    {
        Api = api;

        var window = Application.MainWindow;
        _windowPtr = window.Pointer;
        
        window.SizeChanged += (args) => OnViewportSizeChangedOverride(args.NewValue);
    }

    ~RenderEngine()
    {
        Dispose();
    }

    internal virtual void BeforeDraw() {}

    internal virtual void AfterDraw() {}

    internal virtual void DrawTexture(Texture texture, in Matrix3x2 transform, in Vector3 modulate) {}

    internal virtual void DrawString(Font font, string text, int size, in Matrix3x2 transform, in Vector3 color) {}

    internal virtual void LoadShader(uint shader, ShaderType type, string source) {}

    internal virtual void LoadTexture(uint texture, byte[] data, uint width, uint height) {}

    internal virtual void UnloadShader(uint shader) {}

    internal virtual void UnloadTexture(uint texture) {}

#if DEBUG
    internal virtual void DrawTestRect() {}
#endif

    protected virtual void OnViewportSizeChangedOverride(Vector2Int size) {}

    protected virtual void OnDisposeOverride() {}

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }
        disposed = true;
        
        OnDisposeOverride();
        
        GC.SuppressFinalize(this);
    }
}