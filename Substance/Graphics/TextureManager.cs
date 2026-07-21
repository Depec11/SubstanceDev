using StbImageSharp;
using Substance.Logging;

namespace Substance.Graphics;

internal static class TextureManager
{
    private static uint s_tid = 0;
    private static readonly Dictionary<Uri, TextureCache> s_caches = new();

    internal static uint LoadTexture(Uri uri, out ImageResult? data)
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
            data = ImageResult.FromStream(stream);

            s_caches[uri] = new(++s_tid, data)
            {
                References = 1,
            };

            return s_tid;
        }
        catch (Exception e)
        {
            Log.Warning($"加载纹理 {uri} 失败: {e}");

            return 0;
        }
    }

    internal static void UnloadTexture(uint tid)
    {
        foreach (var (key, cache) in s_caches)

        {
            if (cache.Tid == tid)
            {
                cache.References--;
                if (cache.References == 0)
                {
                    s_caches.Remove(key);

                    cache.Dispose();
                    break;
                }
                break;
            }
        }
    }

    internal class TextureCache : IDisposable
    {
        public uint Tid;
        public uint References = 0;
        public ImageResult Data;

        private bool disposed = false;

        public TextureCache(uint tid, ImageResult data)
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
            if (disposed)
            {
                return;
            }

            disposed = true;

            GC.SuppressFinalize(this);
        }
    }
}