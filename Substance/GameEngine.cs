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
    public event Action Initialized = delegate { };

    private readonly RenderingServer _renderingServer;
    private readonly AudioServer _audioServer;
    private readonly Viewport _viewport;
    private readonly SceneRoot _root = new();
    private readonly Queue<Action> _messageQueue = new();
    private readonly Lock _threadLock = new();
    
    private Node? _splash;
    private bool _disposed = false;

    internal GameEngine(RenderingServer renderingServer)
    {
        _renderingServer = renderingServer;
        _audioServer = new AudioServer();

        _viewport = new();

        _root.OnEnterTree();

        Application.MainWindow.SizeChanged += OnWindowSizeChanged;
    }

    internal void Initialize()
    {
        _ = LoadAudioServer();
        LoadSplash();
    }

    internal void Update(double deltaTime)
    {
        lock(_threadLock)
        {
            while (_messageQueue.Count > 0)
            {
                _messageQueue.Dequeue().Invoke();
            }

            _root.OnUpdate(deltaTime);  
        }
    }

    internal void Render(double deltaTime)
    {
        lock(_threadLock)
        {
            RenderingServer.Current.BeforeDraw();
            _root.OnRendering(deltaTime);
            RenderingServer.Current.AfterDraw();
        }
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

    public void PostMessage(Action action)
    {
        lock(_threadLock)
        {
            _messageQueue.Enqueue(action);
        }
    }

    private void OnWindowSizeChanged(PropertyChangedArgs<Vector2<int>> args)
    {
        _viewport.Size = new Vector2<float>(args.NewValue.X, args.NewValue.Y);

        RenderingServer.Current.UpdateViewportSize(args.NewValue);
    }

    private void LoadSplash()
    {
        var icon = new Texture(new Uri("assets://Substance/Assets/Icon.png"));

        _splash = new Node();

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

        SetScene(_splash);
    }

    private async Task LoadAudioServer()
    {   
        var startTime = DateTime.Now;

        var task = Task.Run(() =>
        {
            _audioServer.MakeAudioEngine(AudioApi.OpenAL);
        });

        await task;

        var endTime = DateTime.Now;

        var duration = endTime - startTime;

        if (duration < TimeSpan.FromSeconds(1.0))
        {
            await Task.Delay(2000 - (int)duration.TotalMilliseconds);
        }

        PostMessage(() =>
        {
            _audioServer.MakeAudioEngine(AudioApi.OpenAL);

            SetScene(null);

            Log.Info($"[{nameof(GameEngine)}] 初始化完成");

            Initialized.Invoke();
        });
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