using SDL3;
using Substance.Core;
using Substance.Logging;

namespace Substance;

public class Window : IDisposable
{
    public event Action<double>? Update;
    public event Action<double>? Render;

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

            SizeChanged?.Invoke(new(oldValue, value));
        } }
    public string Title { get; set; }
    public float RefreshRate { get; }
    
    private bool _disposed = false;
    private bool _isRunning = true;

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

        SDL.GetWindowSize(Pointer, out var width, out var height);

        Size = new(width, height);
        Title = options.Title;
        RefreshRate = GetRefreshRate();
    
        Log.Info($"窗口创建成功: {Title} {Size} {RefreshRate}Hz");

// #if ANDROID
//         SDL.GetDisplayBounds(SDL.GetDisplayForWindow(Pointer), out var displayBounds);

//         Size = new(displayBounds.W, displayBounds.H);
// #endif
    }

    ~Window()
    {
        Dispose();
    }

    public void Exec()
    {
        var deltaTime = 1.0f / RefreshRate;
        var delayTime = (uint)(deltaTime * 1000);

        while (_isRunning)
        {
            while (SDL.PollEvent(out var e))
            {
                switch ((SDL.EventType)e.Type)
                {
                    case SDL.EventType.Quit:
                        _isRunning = false;
                        break;
                    case SDL.EventType.WindowResized:
                        Size = new(e.Window.Data1, e.Window.Data2);
                        break;
                }
            }

            Update?.Invoke(deltaTime);
            Render?.Invoke(deltaTime);

            SDL.Delay(delayTime);
        }

        Dispose();
    }

    private float GetRefreshRate()
    {
        var displayId = SDL.GetDisplayForWindow(Pointer);
        var displayMode = SDL.GetCurrentDisplayMode(displayId);
        return displayMode?.RefreshRate ?? 60.0f;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _isRunning = false;

        SDL.DestroyWindow(Pointer);
        SDL.Quit();

        GC.SuppressFinalize(this);
    }
}