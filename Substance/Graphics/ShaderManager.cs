using Substance.Logging;

namespace Substance.Graphics;

internal static class ShaderManager
{
    private static uint s_sid = 0;
    private static readonly Dictionary<Uri, ShaderCache> s_caches = [];

    internal static uint LoadShader(Shader shader, Uri uri, out string? source)
    {
        if (s_caches.TryGetValue(uri, out var cache))
        {
            cache.References++;
            s_caches[uri] = cache;
            source = cache.Source;
            return cache.Sid;
        }

        source = null;

        var stream = Assets.Open(uri);

        if (stream is null)
        {
            return 0;
        }

        try
        {
            source = Assets.ReadText(uri);

            s_caches[uri] = new(++s_sid, source)
            {
                References = 1,
            };

            RenderingServer.Current.LoadShader(s_sid, shader.Type, source ?? "");

            return s_sid;
        }
        catch (Exception e)
        {
            Log.Warning($"加载着色器 {uri} 失败: {e}");

            return 0;
        }
    }

    internal static void UnloadShader(Shader shader)
    {
        foreach (var (key, cache) in s_caches)
        {
            if (cache.Sid == shader.Sid)
            {
                cache.References--;
                if (cache.References == 0)
                {
                    s_caches.Remove(key);

                    RenderingServer.Current.UnloadShader(cache.Sid);
                    cache.Dispose();
                    break;
                }
                break;
            }
        }
    }

    internal static string? GetSource(uint sid)
    {
        foreach (var (key, cache) in s_caches)
        {
            if (cache.Sid == sid)
            {
                return cache.Source;
            }
        }
        return null;
    }

    internal class ShaderCache : IDisposable
    {
        public uint Sid;
        public uint References = 0;
        public string? Source = null;

        private bool disposed = false;

        public ShaderCache(uint tid, string? source = null)
        {
            Sid = tid;
            Source = source;
        }

        ~ShaderCache()
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