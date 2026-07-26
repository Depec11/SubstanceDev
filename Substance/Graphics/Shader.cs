namespace Substance.Graphics;

public class Shader : IDisposable
{
    public static readonly string MVP = "u_mvp";
    public static readonly string Texture = "u_texture";
    public static readonly string Modulate = "u_modulate";

    internal string? Source => ShaderManager.GetSource(Sid);

    internal readonly uint Sid;
    internal readonly ShaderType Type;

    private bool _disposed = false;

    public Shader(Uri uri)
    {
        Sid = ShaderManager.LoadShader(this, uri, out var _);

        if (Sid == 0)
        {
            throw new IOException($"加载着色器失败: {uri}");
        }

        Type = ShaderType.Vertex;

        var suffix = Path.GetExtension(uri.ToString());
        Type = suffix switch
        {
            ".vert" => ShaderType.Vertex,
            ".frag" => ShaderType.Fragment,
            _ => throw new ArgumentException($"未知的文件类型: {suffix}"),
        };
    }

    ~Shader()
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
        
        ShaderManager.UnloadShader(this);
        
        GC.SuppressFinalize(this);
    }
}