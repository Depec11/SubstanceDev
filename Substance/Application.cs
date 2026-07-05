namespace Substance;

public class Application : IDisposable
{
    public static Application Instance = null!;
    public static Window? MainWindow => Instance._mainWindow;
    public static GameEngine? GameEngine => Instance._gameEngine;

    private readonly Window _mainWindow;
    private readonly GameEngine _gameEngine;
    private bool _disposed;

    public Application(Window mainWindow)
    {
        Instance = this;

        _mainWindow = mainWindow;
        _gameEngine = new GameEngine();

        _mainWindow.Update += _gameEngine.Update;
        _mainWindow.Render += _gameEngine.Render;
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

        _gameEngine.Dispose();
        _mainWindow.Dispose();

        GC.SuppressFinalize(this);
    }
}