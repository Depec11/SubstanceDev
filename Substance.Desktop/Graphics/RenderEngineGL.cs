using SDL3;
using Silk.NET.OpenGL;
using Substance.Graphics;
using Substance.Graphics.TextRendering;
using Substance.Logging;
using Substance.Maths;
using Shader = Substance.Graphics.Shader;
using ShaderType = Substance.Graphics.ShaderType;
using GLShaderType = Silk.NET.OpenGL.ShaderType;

namespace Substance.Desktop.Graphics;

public class RenderEngineGL : RenderEngine
{
    private const string c_platform = "Desktop";
    private const string c_api = "OpenGL";

    private readonly IntPtr _glContext;
    private readonly GL _gl;
    private readonly GLWrapper _glWrapper;
    private readonly TextRenderEngineFT _textRenderEngine;
    
    private readonly Dictionary<uint, uint> _shaderCaches = [];
    private readonly Dictionary<uint, uint> _textureCaches = [];

    private (uint Vao, uint Vbo, uint Ebo) _rectMesh;
    private uint _textureRectProgram;

#if DEBUG
    private uint _vao;
    private uint _vbo;
    private uint _ebo;
    private uint _program;
#endif

    internal RenderEngineGL() : base(GraphicApi.OpenGL)
    {
        _textRenderEngine = new TextRenderEngineFT(new Uri("assets://Substance/Assets/QynFlavorAltCHS-Regular.ttf"), 16);

        var window = Application.MainWindow;

        _glContext = SDL.GLCreateContext(_windowPtr);

        if (_glContext == IntPtr.Zero)
        {
            var error = $"SDL创建{Api}上下文失败: {SDL.GetError()}";
            SDL.LogError(SDL.LogCategory.System, error);
            window.Dispose();
            throw new Exception(error);
        }

        if (!SDL.GLMakeCurrent(_windowPtr, _glContext))
        {
            var error = $"SDL设置{Api}上下文失败: {SDL.GetError()}";
            SDL.LogError(SDL.LogCategory.System, error);
            SDL.GLDestroyContext(_glContext);
            window.Dispose();
            throw new Exception(error);
        }

        SDL.GLSetSwapInterval(1);

        _gl = GL.GetApi(SDL.GLGetProcAddress);

        _glWrapper = new GLWrapper();
        
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        _gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
        _gl.Viewport(0, 0, (uint)window.Size.X, (uint)window.Size.Y);

        Log.Info($"[{nameof(RenderEngineGL)}] 创建成功");

        Initialize();
    }

    protected override void BeforeDrawOverride()
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    protected override void AfterDrawOverride()
    {
        SDL.GLSwapWindow(_windowPtr);
    }

#if DEBUG
    protected unsafe override void DrawTestRectOverride()
    {
        _gl.BindVertexArray(_vao);
        _gl.UseProgram(_program);
        _gl.DrawElements(
            PrimitiveType.Triangles, 
            6, 
            DrawElementsType.UnsignedInt, 
            null
        );
    }
#endif

    protected unsafe override void DrawTextureOverride(uint texture, in Matrix3x2 transform, in Vector3<float> modulate)
    {
        if (_textureRectProgram is 0)
        {
            Log.Warning($"[{nameof(RenderEngineGL)}] 未加载纹理着色器");

            return;
        }

        var vao = _rectMesh.Vao;

        if (vao is 0)
        {
            Log.Warning($"[{nameof(RenderEngineGL)}] 未加载矩形网格");

            return;
        }

        if (!_textureCaches.TryGetValue(texture, out var textureId))
        {
            Log.Warning($"[{nameof(RenderEngineGL)}] 未加载纹理 {texture}");

            return;
        }

        _gl.UseProgram(_textureRectProgram);

        var mvpLoc = _gl.GetUniformLocation(_textureRectProgram, Shader.MVP);

        if (mvpLoc > -1)
        {
            var transformCopy = transform;

            _gl.UniformMatrix3x2(mvpLoc, 1, false, (float*)&transformCopy);
        }

        var texLoc = _gl.GetUniformLocation(_textureRectProgram, Shader.Texture);

        if (texLoc > -1)
        {
            _gl.Uniform1(texLoc, 0);
            _gl.ActiveTexture(TextureUnit.Texture0);
            _gl.BindTexture(TextureTarget.Texture2D, textureId);
        }

        var modulateLoc = _gl.GetUniformLocation(_textureRectProgram, Shader.Modulate);

        if (modulateLoc > -1)
        {
            _gl.Uniform3(modulateLoc, modulate.X, modulate.Y, modulate.Z);
        }

        _gl.BindVertexArray(vao);
        _gl.DrawElements(
            PrimitiveType.Triangles, 
            6, 
            DrawElementsType.UnsignedInt, 
            null
        );

        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, 0);

        _gl.BindVertexArray(0);
    }

    protected override void MeasureStringOverride(string text, uint fontSize, ref Vector2<int> size)
    {
        Vector2<int> startPosition = new(), endPosition = new();
        _textRenderEngine.MesasureString(text, fontSize, ref startPosition, ref endPosition);
        size = endPosition - startPosition;
    }

    protected unsafe override void RenderStringOverride(string text, uint texture, uint fontSize, Color foregroundColor, Color backgroundColor, ref Vector2<int> size)
    {
        byte[] data= [];
        _textRenderEngine.RenderString(text, fontSize, foregroundColor, backgroundColor, ref data, ref size);
    
        var textureId = _textureCaches[texture];

        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, textureId);

        var textureWrapS = (int)TextureWrapMode.Repeat;
        var textureWrapT = (int)TextureWrapMode.Repeat;
        var textureMinFilter = (int)TextureMinFilter.Linear;
        var textureMagFilter = (int)TextureMagFilter.Linear;

        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, ref textureWrapS);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, ref textureWrapT);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, ref textureMinFilter);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, ref textureMagFilter);
        
        fixed (byte* pData = data)
        {
            _gl.TexImage2D(
                TextureTarget.Texture2D,
                0,
                InternalFormat.Rgba,
                (uint)size.X,
                (uint)size.Y,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                pData
            );
        }

        _gl.BindTexture(TextureTarget.Texture2D, 0);
    }

    protected override void DrawStringOverride(uint texture, in Matrix3x2 transform)
    {
        DrawTextureOverride(texture, transform, Vector3<float>.One);
    }

    protected override void LoadShaderOverride(uint shader, ShaderType type, string source)
    {
        if (_shaderCaches.ContainsKey(shader))
        {
            return;
        }

        var id = LoadShader(source, type);
    
        _shaderCaches.Add(shader, id);

        Log.Info($"[{nameof(RenderEngineGL)}] 加载着色器 {shader} 成功");
    }

    protected unsafe override void LoadTextureOverride(uint texture, byte[] data, uint width, uint height)
    {
        if (_textureCaches.ContainsKey(texture))
        {
            return;
        }

        var id = _gl.GenTexture();
        _gl.ActiveTexture(TextureUnit.Texture0);
        _gl.BindTexture(TextureTarget.Texture2D, id);

        fixed (byte* pData = data)
        {
            _gl.TexImage2D(
                TextureTarget.Texture2D,
                0,
                InternalFormat.Rgba,
                width,
                height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                pData
            );
        }

        var textureWrapS = (int)TextureWrapMode.Repeat;
        var textureWrapT = (int)TextureWrapMode.Repeat;
        var textureMinFilter = (int)TextureMinFilter.Linear;
        var textureMagFilter = (int)TextureMagFilter.Linear;

        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapS, ref textureWrapS);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureWrapT, ref textureWrapT);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMinFilter, ref textureMinFilter);
        _gl.TexParameterI(GLEnum.Texture2D, GLEnum.TextureMagFilter, ref textureMagFilter);

        _gl.BindTexture(TextureTarget.Texture2D, 0);

        _textureCaches.Add(texture, id);

        Log.Info($"[{nameof(RenderEngineGL)}] 加载纹理 {texture} 成功");
    }

    protected override void UnloadShaderOverride(uint shader)
    {
        if (_shaderCaches.TryGetValue(shader, out uint value))
        {
            _gl.DeleteShader(value);
            _shaderCaches.Remove(shader);

            Log.Info($"[{nameof(RenderEngineGL)}] 卸载着色器 {shader} 成功");
        }
    }

    protected override void UnloadTextureOverride(uint texture)
    {
        if (_textureCaches.TryGetValue(texture, out uint value))
        {
            _glWrapper.DeleteTexture(value);
            _textureCaches.Remove(texture);
            
            Log.Info($"[{nameof(RenderEngineGL)}] 卸载纹理 {texture} 成功");
        }
    }

    protected override void OnDisposeOverride()
    {
        foreach (var (key, value) in _textureCaches)
        {
            _glWrapper.DeleteTexture(value);
        }

        _textureCaches.Clear();

        foreach (var (key, value) in _shaderCaches)
        {
            _gl.DeleteShader(value);
        }

        _shaderCaches.Clear();

        _glWrapper.DeleteBuffer(_rectMesh.Vbo);
        _glWrapper.DeleteBuffer(_rectMesh.Ebo);
        _glWrapper.DeleteVertexArray(_rectMesh.Vao);

#if DEBUG
        _glWrapper.DeleteBuffer(_vbo);
        _glWrapper.DeleteBuffer(_ebo);
        _glWrapper.DeleteVertexArray(_vao);
        _glWrapper.DeleteProgram(_program);
#endif

        _gl.Dispose();

        SDL.GLDestroyContext(_glContext);

        Log.Info($"[{nameof(RenderEngineGL)}] 销毁成功");
    }

    protected override void OnViewportSizeChangedOverride(Vector2<int> size)
    {
        _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
    }

    private void Initialize()
    {
#if DEBUG
        InitializeDebug();
#endif

        var vao = LoadMesh(
            [
                0.0f, 0.0f, 0.0f, 0.0f,
                1.0f, 0.0f, 1.0f, 0.0f,
                1.0f, 1.0f, 1.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
            ], 
            [
                0u, 1u, 2u,
                0u, 2u, 3u
            ], 
            4, 
            [2, 2],
            out var vbo, 
            out var ebo
        );
        _rectMesh = (vao, vbo, ebo);

        var vertexSource = Assets.ReadText(new Uri($"assets://Substance.{c_platform}/Assets/Shaders/{c_api}/SpriteUnlit.vert"));
        var fragmentSource = Assets.ReadText(new Uri($"assets://Substance.{c_platform}/Assets/Shaders/{c_api}/SpriteUnlit.frag"));
        var vertex = LoadShader(vertexSource ?? "", ShaderType.Vertex);
        var fragment = LoadShader(fragmentSource ?? "", ShaderType.Fragment);
        _textureRectProgram = LoadProgram(vertex, fragment);
    }

    private unsafe uint LoadMesh(float[] vertices, uint[] indices, uint vertexCount, int[] sizes, out uint vbo, out uint ebo)
    {
        var vao = _gl.GenVertexArray();
        _gl.BindVertexArray(vao);

        vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
        _gl.BufferData(
            GLEnum.ArrayBuffer, 
            (nuint)(vertices.Length * sizeof(float)), 
            vertices, 
            GLEnum.StaticDraw
        );

        ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, ebo);
        _gl.BufferData(
            GLEnum.ElementArrayBuffer, 
            (nuint)(indices.Length * sizeof(uint)), 
            indices, 
            GLEnum.StaticDraw
        );

        var strideSize = (uint)vertices.Length / vertexCount * sizeof(float);

        var offset = 0;
        for (uint i = 0; i < sizes.Length; i++)
        {
            _gl.VertexAttribPointer(
                i,
                sizes[i],
                VertexAttribPointerType.Float,
                false,
                strideSize,
                (void*)offset
            );

            _gl.EnableVertexAttribArray(i);

            offset += sizes[i] * sizeof(float);
        }

        _gl.BindVertexArray(0);

        return vao;
    }

    private uint LoadProgram(uint vertex, uint fragment)
    {
        var program = _gl.CreateProgram();
        
        _gl.AttachShader(program, vertex);
        _gl.AttachShader(program, fragment);
        _gl.LinkProgram(program);
        _gl.GetProgram(program, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            Log.Error($"[{nameof(RenderEngineGL)}] 链接程序失败: {_gl.GetProgramInfoLog(program)}");
            _gl.DeleteProgram(program);
            return 0;
        }

        _gl.DetachShader(program, vertex);
        _gl.DetachShader(program, fragment);
        
        return program;
    }

    private uint LoadShader(string source, ShaderType shaderType)
    {
        var id = _gl.CreateShader(SwithToOpenGLShaderType(shaderType));

        _gl.ShaderSource(id, source);
        _gl.CompileShader(id);

        var infoLog = _gl.GetShaderInfoLog(id);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Log.Warning($"[{nameof(RenderEngineGL)}] 编译着色器 类型 {shaderType} 失败: {infoLog} 源码：{source}");
            _gl.DeleteShader(id);
            return 0;
        }

        return id;

        static GLShaderType SwithToOpenGLShaderType(ShaderType type)
        {
            return type switch
            {
                ShaderType.Vertex => GLShaderType.VertexShader,
                ShaderType.Fragment => GLShaderType.FragmentShader,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
            };
        }
    }

#if DEBUG
    private void InitializeDebug()
    {
        var vertices = new float[]
        {
            -0.5f, -0.5f,
            -0.5f,  0.5f,
             0.5f, -0.5f,
             0.5f,  0.5f
        };

        var indices = new uint[]
        {
            0, 1, 2,
            1, 2, 3
        };

        _vao = LoadMesh(vertices, indices, 4, [2], out var vbo, out var ebo);
        _vbo = vbo;
        _ebo = ebo;

        var vertexShader = LoadShader(ShaderSources.VertexShaderSourceGL, ShaderType.Vertex);
        var fragmentShader = LoadShader(ShaderSources.FragmentShaderSourceGL, ShaderType.Fragment);

        _program = _gl.CreateProgram();
        _gl.AttachShader(_program, vertexShader);
        _gl.AttachShader(_program, fragmentShader);
        _gl.LinkProgram(_program);
        _gl.GetProgram(_program, GLEnum.LinkStatus, out var status);
        if (status == 0)
        {
            Log.Error($"链接程序失败: {_gl.GetProgramInfoLog(_program)}");
        }
        
        _gl.DetachShader(_program, vertexShader);
        _gl.DetachShader(_program, fragmentShader);
        _gl.DeleteShader(vertexShader);
        _gl.DeleteShader(fragmentShader);
    }
#endif

    private unsafe class GLWrapper
    {
        private readonly delegate* unmanaged<int, uint*, void> _glDeleteBuffers;
        private readonly delegate* unmanaged<uint, void> _glDeleteVertexArrays;
        private readonly delegate* unmanaged<uint, void> _glDeleteProgram;
        private readonly delegate* unmanaged<uint, uint*, void> _glDeleteTextures;

        public GLWrapper()
        {
            _glDeleteBuffers = (delegate* unmanaged<int, uint*, void>)SDL.GLGetProcAddress("glDeleteBuffers");
            _glDeleteVertexArrays = (delegate* unmanaged<uint, void>)SDL.GLGetProcAddress("glDeleteVertexArrays");
            _glDeleteProgram = (delegate* unmanaged<uint, void>)SDL.GLGetProcAddress("glDeleteProgram");
            _glDeleteTextures = (delegate* unmanaged<uint, uint*, void>)SDL.GLGetProcAddress("glDeleteTextures");
        }

        public void DeleteBuffer(uint buffer)
        {
            _glDeleteBuffers(1, &buffer);
        }

        public void DeleteVertexArray(uint vao)
        {
            _glDeleteVertexArrays(vao);
        }

        public void DeleteProgram(uint program)
        {
            _glDeleteProgram(program);
        }

        public void DeleteTexture(uint texture)
        {
            _glDeleteTextures(1, &texture);
        }
    }
}