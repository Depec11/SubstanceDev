using Substance.Logging;

namespace Substance.Graphics;

public class Texture : IDisposable
{
    public byte[] Data => AudioManager.GetData(Tid).Data;
    public int Width => AudioManager.GetData(Tid).Width;
    public int Height => AudioManager.GetData(Tid).Height;

    internal readonly uint Tid;

    private bool _disposed = false;

    public Texture()
    {
        Tid = AudioManager.RequestTid();
    }

    public Texture(Uri uri)
    {
        Tid = AudioManager.LoadTexture(uri, out var data);
        
        if (data is null)
        {
            Log.Warning($"创建纹理 {uri} 失败");
            return;
        }
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
        
        AudioManager.UnloadTexture(this);
        
        GC.SuppressFinalize(this);
    }
}