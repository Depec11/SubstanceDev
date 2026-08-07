using System.Runtime.CompilerServices;

namespace Substance.Audio;

public class AudioEngine : IDisposable
{
    public AudioApi Api { get; }

    private bool _disposed = false;

    internal AudioEngine()
    {
        Api = AudioApi.None;
    }

    protected AudioEngine(AudioApi api = AudioApi.None)
    {
        Api = api;
    }

    ~AudioEngine()
    {
        Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void LoadSound(uint sid, AudioData data) => LoadSoundOverride(sid, data);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void UnloadSound(uint sid) => UnloadSoundOverride(sid);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void PlaySound(uint sound) => PlaySoundOverride(sound);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void PauseSound(uint sound) => PauseSoundOverride(sound);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void StopSound(uint sound) => StopSoundOverride(sound);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void SetSoundIsLooping(uint sound, bool isLooping) => SetSoundIsLoopingOverride(sound, isLooping);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool GetSoundIsLooping(uint sound) => GetSoundIsLoopingOverride(sound);

    protected virtual void LoadSoundOverride(uint sid, AudioData data) {}

    protected virtual void UnloadSoundOverride(uint sid) {}

    protected virtual void PlaySoundOverride(uint sound) {}

    protected virtual void PauseSoundOverride(uint sound) {}

    protected virtual void StopSoundOverride(uint sound) {}

    protected virtual void SetSoundIsLoopingOverride(uint sound, bool isLooping) {}

    protected virtual bool GetSoundIsLoopingOverride(uint sound) => false;

    protected virtual void OnDisposeOverride() {}

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
    
        OnDisposeOverride();

        GC.SuppressFinalize(this);
    }
}