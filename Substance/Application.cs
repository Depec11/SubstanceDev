namespace Substance;

public class Application : IDisposable
{
    public static Application Instance = null!;
    public static Window? MainWindow => Instance._mainWindow;

    private Window _mainWindow;
    private bool _disposed;

    public Application(Window mainWindow)
    {
        Instance = this;

        _mainWindow = mainWindow;
    }

    ~Application()
    {
        Dispose();
    }

    public void Exec()
    {
        _mainWindow?.Exec();

        Dispose();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _mainWindow.Dispose();

        GC.SuppressFinalize(this);
    }
}