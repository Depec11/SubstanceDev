using Substance;
using Substance.Audio;

namespace Test;

public class MainGameLoop : GameLoop
{
    private SoundSource? _soundSource;

    protected override void OnInitializedOverride()
    {
        Console.WriteLine("GameLoop Initialized");

        // _soundSource = new SoundSource(new Uri("assets://Substance/Assets/Theme.ogg"));
        // AudioServer.Current.PlaySound(_soundSource!.Sid);
    }
}
