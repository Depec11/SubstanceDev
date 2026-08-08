using NVorbis;
using Substance.Logging;

namespace Substance.Audio;

internal static class AudioManager
{
    private static uint s_sid = 0;
    private static readonly Dictionary<Uri, AudioCache> s_caches = [];

    internal static uint LoadSound(Uri uri, out AudioData? data)
    {
        if (s_caches.TryGetValue(uri, out var cache))
        {
            data = cache.Data;
            cache.References++;
            s_caches[uri] = cache;
            return cache.Sid;
        }

        data = null;

        var stream = Assets.Open(uri);

        if (stream is null)
        {
            return 0;
        }

        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;

        var headerBytes = new byte[4];
        var readCount = memoryStream.Read(headerBytes, 0, 4);
        memoryStream.Position = 0;
        var isOggS = readCount == 4 && headerBytes[0] == 0x4F && headerBytes[1] == 0x67 && headerBytes[2] == 0x67 && headerBytes[3] == 0x53;

        using var vorbisReader = new VorbisReader(memoryStream);
        var sampleRate = vorbisReader.SampleRate;
        var channels = vorbisReader.Channels;

        var buffer = new float[vorbisReader.TotalSamples * channels];
        vorbisReader.ReadSamples(buffer, 0, buffer.Length);

        var pcmData = new byte[buffer.Length * 2];
        for (var i = 0; i < buffer.Length; i++)
        {
            var sample = (short)(Math.Clamp(buffer[i], -1f, 1f) * 32767f);
            pcmData[i * 2] = (byte)(sample & 0xff);
            pcmData[i * 2 + 1] = (byte)((sample >> 8) & 0xff);
        }

        data = new AudioData(sampleRate, channels, 16, pcmData);

        s_caches[uri] = new(++s_sid, data)
        {
            References = 1,
        };

        AudioServer.Current.LoadSound(s_sid, data);

        return s_sid;
}

    internal static void UnloadSound(uint sid)
    {
        foreach (var (key, cache) in s_caches)
        {
            if (cache.Sid == sid)
            {
                cache.References--;
                if (cache.References == 0)
                {
                    s_caches.Remove(key);
                    
                    AudioServer.Current.UnloadSound(cache.Sid);
                    cache.Dispose();
                    break;
                }
                break;
            }
        }
    }

    internal static AudioData GetData(uint sid)
    {
        foreach (var (_, cache) in s_caches)
        {
            if (cache.Sid == sid)
            {
                return cache.Data;
            }
        }

        return AudioData.Empty;
    }

    internal static void SetIsLooping(uint sid, bool isLooping)
    {
        AudioServer.Current.SetSoundIsLooping(sid, isLooping);
    }

    internal class AudioCache : IDisposable
    {
        public readonly uint Sid;
        public readonly AudioData Data;
        public uint References = 0;
        private bool _disposed = false;

        public AudioCache(uint sid, AudioData data)
        {
            Sid = sid;
            Data = data;
        }

        ~AudioCache()
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

            GC.SuppressFinalize(this);
        }
    }
}