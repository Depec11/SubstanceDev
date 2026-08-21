using Substance.Audio;
using Substance.Core;
using Substance.Graphics;
using Substance.Logging;
using Substance.Maths;
using Substance.Nodes;
using Substance.Nodes.Canvas;

namespace Substance;

public class GameEngine : IDisposable
{
    public event Action Intialized = delegate { };

    private readonly RenderingServer _renderingServer;
    private readonly AudioServer _audioServer;
    private readonly Viewport _viewport;
    private readonly SceneRoot _root = new();
    
    private Node? _splash;
    private bool _disposed = false;

    internal GameEngine(RenderingServer renderingServer)
    {
        _renderingServer = renderingServer;
        _audioServer = new AudioServer();

        _viewport = new();

        _root.OnEnterTree();

        Application.MainWindow.SizeChanged += OnWindowSizeChanged;

        Log.Info($"[{nameof(GameEngine)}] 初始化完成");
    }

    internal async Task Initialize()
    {   
        var startTime = DateTime.Now;

        var task = Task.Run(() =>
        {
            _audioServer.MakeAudioEngine(AudioApi.OpenAL);
        });

        var icon = new Texture(new Uri("assets://Substance/Assets/Icon.png"));

        _splash = new Node();

        _splash.SetParent(_root);

        var spriteRenderer = new SpriteRenderer
        {
            Texture = icon,
            Transform =
            {
                Position = new Vector2<float>(400.0f, 300.0f),
                Pivot = new Vector2<float>(0.5f, 0.5f),
            }
        };
        spriteRenderer.SetParent(_splash);

        var label = new Label
        {
            Text = "单质 - Substance",
            Transform =
            {
                Position = new Vector2<float>(400.0f, 300.0f + 96.0f),
                Pivot = new Vector2<float>(0.5f, 0.5f),
            },
            IsInScene = true,
            Font =
            {
                Size = 12,
            }
        };
        label.SetParent(_splash);

        var camera = new Camera
        {
            Transform =
            {
                Position = new Vector2<float>(400.0f, 300.0f),
            }
        };
        camera.SetParent(_splash);

        Vector2<int> textSize = new();

        await task;

        var endTime = DateTime.Now;

        var duration = endTime - startTime;

        if (duration < TimeSpan.FromSeconds(1.0))
        {
            await Task.Delay(1000 - (int)duration.TotalMilliseconds);
        }

        SetScene(null);
        Intialized.Invoke();

        // _audioServer.MakeAudioEngine(AudioApi.OpenAL);

        // _soundSource = new SoundSource(new Uri("assets://Substance/Assets/Theme.ogg"));
        // AudioServer.Current.PlaySound(_soundSource.Sid);

        Log.Info($"[{nameof(GameEngine)}] 初始化完成");
    }

    internal void Update(double deltaTime)
    {
        _root.OnUpdate(deltaTime);  
    }

    internal void Render(double deltaTime)
    {
        RenderingServer.Current.BeforeDraw();
        _root.OnRendering(deltaTime);
        
        RenderingServer.Current.AfterDraw();
    }

    internal void MakeRenderEngine(GraphicApi api)
    {
        _renderingServer.MakeRenderEngine(api);
    }

    public void SetScene(Node? scene)
    {
        _root.ClearChildren();
        scene?.SetParent(_root);
    }

    private void OnWindowSizeChanged(PropertyChangedArgs<Vector2<int>> args)
    {
        _viewport.Size = new Vector2<float>(args.NewValue.X, args.NewValue.Y);

        RenderingServer.Current.UpdateViewportSize(args.NewValue);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        Application.MainWindow.SizeChanged -= OnWindowSizeChanged;

        _splash?.Dispose();

        _root.ExitTree();

        _renderingServer.Dispose();
    
        GC.SuppressFinalize(this);
    }
}