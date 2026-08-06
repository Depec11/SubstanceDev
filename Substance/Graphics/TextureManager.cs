using StbImageSharp;
using Substance.Logging;

namespace Substance.Graphics;

internal static class AudioManager
{
    private static uint s_tid = 0;
    private static readonly Dictionary<Uri, TextureCache> s_caches = [];

    internal static uint LoadTexture(Uri uri, out TextureData? data)
    {
        if (s_caches.TryGetValue(uri, out var cache))
        {
            data = cache.Data;
            cache.References++;
            s_caches[uri] = cache;
            return cache.Tid;
        }

        data = null;

        var stream = Assets.Open(uri);

        if (stream is null)
        {
            return 0;
        }

        try
        {
            var imageResult = ImageResult.FromStream(stream);

            data = new TextureData(imageResult.Width, imageResult.Height, imageResult.Data);

            s_caches[uri] = new(++s_tid, data);

            s_caches[uri] = new(++s_tid, data)
            {
                References = 1,
            };

            RenderingServer.Current.LoadTexture(s_tid, data.Data, (uint)data.Width, (uint)data.Height);

            return s_tid;
        }
        catch (Exception e)
        {
            Log.Warning($"加载纹理 {uri} 失败: {e}");

            return 0;
        }
    }

    internal static uint RequestTid()
    {
        ++s_tid;

        RenderingServer.Current.LoadTexture(s_tid, [], 0, 0);

        return s_tid;
    }

    internal static void UnloadTexture(Texture texture)
    {
        foreach (var (key, cache) in s_caches)
        {
            if (cache.Tid == texture.Tid)
            {
                cache.References--;
                if (cache.References == 0)
                {
                    s_caches.Remove(key);
                    
                    RenderingServer.Current.UnloadTexture(cache.Tid);
                    cache.Dispose();
                    break;
                }
                break;
            }
        }
    }

    internal static void FreeTid(uint tid)
    {
        RenderingServer.Current.UnloadTexture(tid);
    }

    internal static TextureData GetData(uint tid)
    {
        foreach (var (_, cache) in s_caches)
        {
            if (cache.Tid == tid)
            {
                return cache.Data;
            }
        }

        return TextureData.Empty;
    }

    internal class TextureCache : IDisposable
    {
        public readonly uint Tid;
        public uint References = 0;
        public readonly TextureData Data;

        private bool _disposed = false;

        public TextureCache(uint tid, TextureData data)
        {
            Tid = tid;
            Data = data;
        }

        ~TextureCache()
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