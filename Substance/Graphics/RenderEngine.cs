using System.Runtime.CompilerServices;
using Substance.Maths;

namespace Substance.Graphics;

public class RenderEngine : IDisposable
{
    public GraphicApi Api { get; }

    protected readonly IntPtr _windowPtr;
    
    private bool disposed = false;
    
    internal RenderEngine()
    {
        Api = GraphicApi.None;

        var window = Application.MainWindow;
        _windowPtr = window.Pointer;
        
        window.SizeChanged += (args) => OnViewportSizeChangedOverride(args.NewValue);
    }
    
    protected RenderEngine(GraphicApi api = GraphicApi.None)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void BeforeDraw() => BeforeDrawOverride();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void AfterDraw() => AfterDrawOverride();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void DrawTexture(uint texture, in Matrix3x2 transform, in Vector3<float> modulate) => DrawTextureOverride(texture, transform, modulate);

    // internal void DrawString(uint font, string text, int size, in Matrix3x2 transform, in Vector3 color) {}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void LoadShader(uint shader, ShaderType type, string source) => LoadShaderOverride(shader, type, source);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void LoadTexture(uint texture, byte[] data, uint width, uint height) => LoadTextureOverride(texture, data, width, height);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void UnloadShader(uint shader) => UnloadShaderOverride(shader);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void UnloadTexture(uint texture) => UnloadTextureOverride(texture);

    protected virtual void BeforeDrawOverride() {}

    protected virtual void AfterDrawOverride() {}

    protected virtual void DrawTextureOverride(uint texture, in Matrix3x2 transform, in Vector3<float> modulate) {}

    protected virtual void LoadShaderOverride(uint shader, ShaderType type, string source) {}

    protected virtual void LoadTextureOverride(uint texture, byte[] data, uint width, uint height) {}

    protected virtual void UnloadShaderOverride(uint shader) {}

    protected virtual void UnloadTextureOverride(uint texture) {}

#if DEBUG
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void DrawTestRect() => DrawTestRectOverride();

    protected virtual void DrawTestRectOverride() {}
#endif

    protected virtual void OnViewportSizeChangedOverride(Vector2<int> size) {}

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