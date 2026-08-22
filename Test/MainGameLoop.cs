using Substance;
using Substance.Audio;
using Substance.Nodes;

namespace Test;

public class MainGameLoop : GameLoop
{
    // private SoundSource? _soundSource;

    protected override void OnInitializedOverride()
    {
        Console.WriteLine("GameLoop Initialized");

        var audioSource = new AudioSource
        {
            Source = new SoundSource(new Uri("assets://Substance/Assets/Theme.ogg")),
            IsLooping = true,
        };

        Application.GameEngine.SetScene(audioSource);

        audioSource.Play();


        // _soundSource = new SoundSource(new Uri("assets://Substance/Assets/Theme.ogg"));
        // AudioServer.Current.PlaySound(_soundSource!.Sid);
    }
}
