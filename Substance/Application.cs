using Substance.Graphics;

namespace Substance;

public class Application : IDisposable
{
    public static Application Instance = null!;
    public static Window MainWindow => Instance._mainWindow;
    public static GameEngine GameEngine => Instance._gameEngine;

    private readonly Window _mainWindow;
    private readonly GameEngine _gameEngine;
    private bool _disposed = false;

    public Application(Func<GraphicApi, RenderEngine> createRenderEngine, WindowOptions? options = null)
    {
        Instance = this;

        options ??= new WindowOptions();

        _mainWindow = new(options);

        _gameEngine = new GameEngine(new RenderingServer(createRenderEngine));
        _gameEngine.MakeRenderEngine(options.GraphicApi);

        _mainWindow.Update += _gameEngine.Update;
        _mainWindow.Render += _gameEngine.Render;

        OnCreatedOverride();
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

    protected virtual void OnCreatedOverride() {}

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