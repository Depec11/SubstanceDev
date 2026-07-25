using Substance.Logging;

namespace Substance.Graphics;

public class Texture : IDisposable
{
    internal readonly uint Tid;
    public readonly Vector2Int Size;

    private bool disposed = false;

    public Texture(Uri uri)
    {
        Tid = TextureManager.LoadTexture(uri, out var data);
        
        if (data is null)
        {
            Log.Warning($"创建纹理 {uri} 失败");
            return;
        }

        Size = new Vector2Int(data.Width, data.Height);
    }

    ~Texture()
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
        
        TextureManager.UnloadTexture(Tid);
        
        GC.SuppressFinalize(this);
    }
}