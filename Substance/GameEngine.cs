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
    private static readonly Vector3<float> s_modulate = Vector3<float>.One;

    private readonly RenderingServer _renderingServer;
    private readonly AudioServer _audioServer;
    private readonly Viewport _viewport;
    private readonly SceneRoot _root = new();
    
    private Texture? _icon;
    private Matrix3x2 _iconMatrix;
    private Texture? _text;
    private Matrix3x2 _textMatrix;
    private SoundSource? _soundSource;
    private bool _disposed = false;

    internal GameEngine(RenderingServer renderingServer)
    {
        _renderingServer = renderingServer;
        _audioServer = new AudioServer();

        _viewport = new();

        _root.OnEnterTree();

        Application.MainWindow.SizeChanged += (args) =>
        {
            // _matrix = Matrix3x2.Make(new Vector2<float>(args.NewValue.X, args.NewValue.Y) / 2, 0, new Vector2<float>(_icon!.Width, _icon!.Height));
            _viewport.Size = new Vector2<float>(args.NewValue.X, args.NewValue.Y);
        };

        Log.Info($"[{nameof(GameEngine)}] 初始化完成");
    }

    internal async Task Initialize()
    {   
        var task = Task.Run(() =>
        {
            _audioServer.MakeAudioEngine(AudioApi.OpenAL);
        });

        _icon = new Texture(new Uri("assets://Substance/Assets/Icon.png"));
        _text = new Texture();

        var spriteRenderer = new SpriteRenderer
        {
            Texture = _icon,
            Color = Color.White
        };

        var label = new Label
        {
            Text = "单质"
        };

        spriteRenderer.SetParent(_root);
        label.SetParent(_root);

        Vector2<int> textSize = new();

        RenderingServer.Current.RenderString("单质", _text.Tid, 16, new Color(255, 255, 255, 255), new Color(100, 149, 237, 255), ref textSize);

        if (OperatingSystem.IsAndroid())
        {
            _iconMatrix = Matrix3x2.Create(Vector2<float>.Zero, new Vector2<float>(2), 0, new Vector2<float>(_icon.Width, _icon.Height));
            _textMatrix = Matrix3x2.Create(Vector2<float>.Zero, new Vector2<float>(2), 0, new Vector2<float>(textSize.X, textSize.Y));

        }
        else
        {
            _iconMatrix = Matrix3x2.Make(Vector2<float>.Zero, 0, new Vector2<float>(_icon.Width, _icon.Height));
            _textMatrix = Matrix3x2.Make(Vector2<float>.Zero, 0, new Vector2<float>(textSize.X, textSize.Y));
        }

        await task;

        _audioServer.MakeAudioEngine(AudioApi.OpenAL);

        _soundSource = new SoundSource(new Uri("assets://Substance/Assets/Theme.ogg"));
        AudioServer.Current.PlaySound(_soundSource.Sid);

        Log.Info($"[{nameof(GameEngine)}] 初始化完成");
    }

    internal void Update(double deltaTime)
    {
        _root.OnUpdate(deltaTime);  
    }

    internal void Render(double deltaTime)
    {
        RenderingServer.Current.BeforeDraw();
#if DEBUG
        RenderingServer.Current.DrawTestRect();
#endif
        // RenderingServer.Current.DrawTexture(_icon!.Tid, _viewport.GetSvp(_iconMatrix), s_modulate);
        // RenderingServer.Current.DrawString(_text!.Tid, _viewport.GetSvp(_textMatrix));
        _root.OnRendering(deltaTime);
        
        RenderingServer.Current.AfterDraw();
    }

    internal void MakeRenderEngine(GraphicApi api)
    {
        _renderingServer.MakeRenderEngine(api);
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        _root.ExitTree();

        _icon?.Dispose();
        _text?.Dispose();
        _soundSource?.Dispose();

        _renderingServer.Dispose();
    
        GC.SuppressFinalize(this);
    }
}