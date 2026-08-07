using Substance.Logging;

namespace Substance.Audio;

public class AudioServer : IDisposable
{
    public static AudioEngine Current { get; private set; } = new AudioEngine();

    private bool _disposed = false;
    
    internal AudioServer()
    {
    }

    ~AudioServer()
    {
        Dispose();
    }

    internal void MakeAudioEngine(AudioApi api)
    {
        if (api == Current.Api)
        {
            return;
        }

        Current.Dispose();

        Current = api switch
        {
            AudioApi.OpenAL => new AudioEngineAL(),
            _ => new AudioEngine(),
        };

        Log.Info($"[{nameof(AudioServer)}] 引擎已切换为 {api}");
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        
        Current.Dispose();
        
        GC.SuppressFinalize(this);
    }
}
