using Substance;
using Substance.Audio;
using Substance.Core;
using Substance.Graphics;
using Substance.Maths;
using Substance.Nodes;
using Substance.Nodes.Canvas;

namespace Test;

public class MainGameLoop : GameLoop
{
    private AudioSource? _audioSource;
    private Button? _button;

    ~MainGameLoop()
    {
        _button?.Clicked -= OnButtonClicked;

        Application.MainWindow.Resized -= OnViewportResized;
    }

    protected override void OnInitializedOverride()
    {
        _audioSource = new AudioSource
        {
            Source = new SoundSource(new Uri("assets://Substance/Assets/Theme.ogg")),
            IsLooping = true,
        };

        _button = new Button
        {
            Transform =
            {
                Position = Viewport.Current.Size / 2.0f,
                Origin = new Vector2<float>(0.5f),
            },
            Size = new Vector2<float>(128, 64),
            Text = "播放",
            // IsInScene = true,
            // FontSize = 32,
        };

        _button.Clicked += OnButtonClicked;

        _button.SetParent(_audioSource);

        Application.GameEngine.SetScene(_audioSource);

        Application.MainWindow.Resized += OnViewportResized;
    }

    private void OnButtonClicked(Button button)
    {
        if (_audioSource is null)
        {
            return;
        }

        if (_audioSource.IsPlaying)
        {
            _audioSource.Pause();
            button.Text = "播放";
        }
        else
        {
            _audioSource.Play();
            button.Text = "暂停";
        }
    }

    private void OnViewportResized(PropertyChangedArgs<Vector2<int>> args)
    {
        _button?.Transform.Position = new Vector2<float>(args.NewValue.X / 2.0f, args.NewValue.Y / 2.0f);
    }
}