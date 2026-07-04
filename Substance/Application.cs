using SDL3;

namespace Substance;

public class Application : IDisposable
{
    public static IntPtr WindowPtr => _window;

    private static IntPtr _window;
    private bool _disposed;

    public Application(WindowOptions? options = null)
    {
        options ??= new WindowOptions();

        if (!SDL.Init(SDL.InitFlags.Video | SDL.InitFlags.Events))
        {
            var error = $"SDL初始化失败: {SDL.GetError()}";
            SDL.LogError(SDL.LogCategory.System, error);
            throw new Exception(error);
        }

        _window = SDL.CreateWindow(options.Title, options.Size.X, options.Size.Y, 
                                   SDL.WindowFlags.OpenGL | SDL.WindowFlags.HighPixelDensity | 
                                   (OperatingSystem.IsAndroid() ? 0 : SDL.WindowFlags.Resizable));

        if (_window == IntPtr.Zero)
        {
            var error = $"SDL创建窗口失败: {SDL.GetError()}";
            SDL.LogError(SDL.LogCategory.System, error);
            SDL.Quit();
            throw new Exception(error);
        }
    }

    ~Application()
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
                if ((SDL.EventType)e.Type is SDL.EventType.Quit)
                {
                    isRunning = false;
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

        SDL.DestroyWindow(_window);
        SDL.Quit();

        GC.SuppressFinalize(this);
    }
}