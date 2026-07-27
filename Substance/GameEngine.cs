using Substance.Graphics;
using Substance.Maths;

namespace Substance;

public class GameEngine : IDisposable
{
    private readonly RenderingServer _renderEngineManager;

    private readonly Viewport _viewport;
    private bool _disposed = false;
    
    private Texture? _icon;
    private Matrix3x2 _matrix;
    private static readonly Vector3<float> s_modulate = Vector3<float>.One;

    internal GameEngine(RenderingServer renderingServer)
    {
        _renderEngineManager = renderingServer;

        _viewport = new();

        Application.MainWindow.SizeChanged += (args) =>
        {
            // _matrix = Matrix3x2.Make(new Vector2<float>(args.NewValue.X, args.NewValue.Y) / 2, 0, new Vector2<float>(_icon!.Width, _icon!.Height));
            _viewport.Size = new Vector2<float>(args.NewValue.X, args.NewValue.Y);
        };
    }

    internal void Initialize()
    {
        _icon = new Texture(new Uri("assets://Substance/Assets/Icon.png"));
    
        // _matrix = Matrix3x2.Make(new Vector2<float>(Application.MainWindow.Size.X, Application.MainWindow.Size.Y) / 2, 0, new Vector2<float>(_icon.Width, _icon.Height));
        if (OperatingSystem.IsAndroid())
        {
            _matrix = Matrix3x2.Make(Vector2<float>.Zero, 0, new Vector2<float>(2), new Vector2<float>(_icon.Width, _icon.Height));
        }
        else
        {
            _matrix = Matrix3x2.Make(Vector2<float>.Zero, 0, new Vector2<float>(_icon.Width, _icon.Height));
        }
    }

    internal void Update(double deltaTime) {}

    internal void Render(double deltaTime)
    {
        RenderingServer.Current.BeforeDraw();
#if DEBUG
        // RenderingServer.Current.DrawTestRect();
#endif
        RenderingServer.Current.DrawTexture(_icon!.Tid, _viewport.GetSvp(_matrix), s_modulate);
        RenderingServer.Current.AfterDraw();
    }

    internal void MakeRenderEngine(GraphicApi api)
    {
        _renderEngineManager.MakeRenderEngine(api);
    
        Initialize();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _icon?.Dispose();

        _renderEngineManager.Dispose();
    
        GC.SuppressFinalize(this);
    }
}