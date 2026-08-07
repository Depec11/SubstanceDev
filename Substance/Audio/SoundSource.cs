using Substance.Logging;

namespace Substance.Audio;

public class SoundSource : IDisposable
{
    internal readonly uint Sid;

    internal bool IsLooping { get; set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            AudioManager.SetIsLooping(Sid, value);
        } } = false;

    private bool _disposed = false;

    public SoundSource(Uri uri)
    {
        Sid = AudioManager.LoadSound(uri, out var data);

        if (data is null)
        {
            Log.Warning($"创建音效 {uri} 失败");
            return;
        }
    }

    ~SoundSource()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        AudioManager.UnloadSound(Sid);

        GC.SuppressFinalize(this);
    }
}
