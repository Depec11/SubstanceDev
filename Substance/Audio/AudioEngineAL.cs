using Silk.NET.OpenAL;
using Substance.Logging;

namespace Substance.Audio;

public class AudioEngineAL : AudioEngine
{
    private readonly ALContext _alc;
    private readonly AL _al;
    private readonly Dictionary<uint, (uint Buffer, uint Source)> _soundCaches = [];

    public unsafe AudioEngineAL() : base(AudioApi.OpenAL)
    {
        _alc = ALContext.GetApi();
        _al = AL.GetApi();

        var device = _alc.OpenDevice(null);

        if (device is null)
        {
            Log.Error($"[{nameof(AudioEngineAL)}] 初始化失败：无法打开设备");
        
            return;
        }

        var context = _alc.CreateContext(device, null);

        if (context is null)
        {
            Log.Error($"[{nameof(AudioEngineAL)}] 初始化失败：无法创建上下文");
        
            return;
        }

        _alc.MakeContextCurrent(context);
        
        Log.Info($"[{nameof(AudioEngineAL)}] 初始化成功");
    }

    protected unsafe override void LoadSoundOverride(uint sid, AudioData data)
    {
        if (_soundCaches.ContainsKey(sid))
        {
            return;
        }

        var buffer = _al.GenBuffer();
        var source = _al.GenSource();

        BufferFormat format;

        switch(data.Channels, data.BitsPerSample)
        {
            case (1, 8):
                format = BufferFormat.Mono8;
                break;
            case (1, 16):
                format = BufferFormat.Mono16;
                break;
            case (2, 8):
                format = BufferFormat.Stereo8;
                break;
            case (2, 16):
                format = BufferFormat.Stereo16;
                break;
            default:
                Log.Warning($"[{nameof(AudioEngineAL)}] 加载音频失败：不支持的格式 ({data.Channels}通道, {data.BitsPerSample}位)");
                return;
        }

        fixed (byte* pData = data.Data)
        {
            _al.BufferData(buffer, format, pData, data.Data.Length, data.SampleRate);
        }

        _al.SetSourceProperty(source, SourceInteger.Buffer, buffer);
        _al.SetSourceProperty(source, SourceFloat.Gain, 1.0f);
        _al.SetSourceProperty(source, SourceBoolean.Looping, false);

        _soundCaches.Add(sid, (buffer, source));

        Log.Info($"[{nameof(AudioEngineAL)}] 加载音频成功");
    }

    protected override void UnloadSoundOverride(uint sid)
    {
        if (_soundCaches.TryGetValue(sid, out var cache))
        {
            _al.DeleteBuffer(cache.Buffer);
            _al.DeleteSource(cache.Source);
            _soundCaches.Remove(sid);

            Log.Info($"[{nameof(AudioEngineAL)}] 卸载音频成功");
        }
    }

    protected override void PlaySoundOverride(uint sound)
    {
        if (_soundCaches.TryGetValue(sound, out var cache))
        {
            _al.SourcePlay(cache.Source);
        }
    }

    protected override void PauseSoundOverride(uint sound)
    {
        if (_soundCaches.TryGetValue(sound, out var cache))
        {
            _al.SourcePause(cache.Source);
        }
    }

    protected override void StopSoundOverride(uint sound)
    {
        if (_soundCaches.TryGetValue(sound, out var cache))
        {
            _al.SourceStop(cache.Source);
        }
    }

    protected override void SetSoundIsLoopingOverride(uint sound, bool isLooping)
    {
        if (_soundCaches.TryGetValue(sound, out var cache))
        {
            _al.SetSourceProperty(cache.Source, SourceBoolean.Looping, isLooping);
        }
    }

    protected override bool GetSoundIsLoopingOverride(uint sound)
    {
        if (_soundCaches.TryGetValue(sound, out var cache))
        {
            _al.GetSourceProperty(cache.Source, SourceBoolean.Looping, out bool value);

            return value;
        }

        return false;
    }

    protected unsafe override void OnDisposeOverride()
    {
        foreach (var (_, cache) in _soundCaches)
        {
            _al.DeleteBuffer(cache.Buffer);
            _al.DeleteSource(cache.Source);
        }

        _soundCaches.Clear();

        var context = _alc.GetCurrentContext();

        if (context is not null)
        {
            var device = _alc.GetContextsDevice(context);

            _alc.MakeContextCurrent(null);
            _alc.DestroyContext(context);

            if (device is not null)
            {
                _alc.CloseDevice(device);
            }
        }

        Log.Info($"[{nameof(AudioEngineAL)}] 释放成功");
    }
}