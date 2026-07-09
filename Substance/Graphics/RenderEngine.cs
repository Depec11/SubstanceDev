using System.Runtime.CompilerServices;

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
        
        window.SizeChanged += (args) =>
        {
            Console.WriteLine($"窗口大小改变: {args.NewValue}");
            OnViewportSizeChangedOverride(args.NewValue);
        };
    }

    ~RenderEngine()
    {
        Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void BeginDraw()
    {
        BeforeDrawOverride();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void EndDraw()
    {
        AfterDrawOverride();
    }

    protected virtual void BeforeDrawOverride() {}

    protected virtual void AfterDrawOverride() {}

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