using SDL3;
using Substance.Core;
using Substance.Logging;

namespace Substance;

public class Window : IDisposable
{
    public event Action<PropertyChangedArgs<Vector2Int>>? SizeChanged;

    public IntPtr Pointer { get; }
    public Vector2Int Size { get; set
        {
            if (field == value)
            {
                return;
            }

            var oldValue = field;
            field = value;

            SDL.SetWindowSize(Pointer, value.X, value.Y);

            Log.Info($"窗口大小 从 {oldValue} 变更为 {value}");

            SizeChanged?.Invoke(new(value, oldValue));
        } }
    public string Title { get; set; }
    
    private bool _disposed;

    public Window(WindowOptions? options = null)
    {
        options ??= new WindowOptions();

        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Events))
        {
            var error = $"SDL初始化失败: {SDL.GetError()}";
            SDL.LogError(SDL.LogCategory.System, error);
            throw new Exception(error);
        }

        Pointer = SDL.CreateWindow(options.Title, options.Size.X, options.Size.Y, 
                                   SDL.WindowFlags.OpenGL | SDL.WindowFlags.HighPixelDensity | 
                                   (OperatingSystem.IsAndroid() ? 0 : SDL.WindowFlags.Resizable));

        if (Pointer == IntPtr.Zero)
        {
            var error = $"SDL创建窗口失败: {SDL.GetError()}";
            SDL.LogError(SDL.LogCategory.System, error);
            SDL.Quit();
            throw new Exception(error);
        }

        Size = options.Size;
        Title = options.Title;
    }

    ~Window()
    {
        Dispose();
    }

    public void Exec()
    {
        var isRunning = true;

        while (isRunning)
        {
            while (SDL.PollEvent(out var e))
            {
                switch ((SDL.EventType)e.Type)
                {
                    case SDL.EventType.Quit:
                        isRunning = false;
                        break;
                    case SDL.EventType.WindowResized:
                        Size = new(e.Window.Data1, e.Window.Data2);
                        break;
                }
            }
        }

        Dispose();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        SDL.DestroyWindow(Pointer);
        SDL.Quit();

        GC.SuppressFinalize(this);
    }
}