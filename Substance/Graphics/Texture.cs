using Substance.Logging;

namespace Substance.Graphics;

public class Texture : IDisposable
{
    public byte[] Data => TextureManager.GetData(Tid);

    internal readonly uint Tid;
    public readonly uint Width;
    public readonly uint Height;

    private bool _disposed = false;

    internal Texture(uint tid)
    {
        Tid = tid;
    }

    public Texture(Uri uri)
    {
        Tid = TextureManager.LoadTexture(this, uri, out var data);
        
        if (data is null)
        {
            Log.Warning($"创建纹理 {uri} 失败");
            return;
        }

        Width = (uint)data.Width;
        Height = (uint)data.Height;
    }

    ~Texture()
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
        
        TextureManager.UnloadTexture(this);
        
        GC.SuppressFinalize(this);
    }
}