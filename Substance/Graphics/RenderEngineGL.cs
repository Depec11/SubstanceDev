#if !ANDROID

using SDL3;
using Silk.NET.OpenGL;
using Substance.Logging;

namespace Substance.Graphics;

public class RenderEngineGL : RenderEngine
{
    private unsafe class GLWrapper
    {
        private readonly delegate* unmanaged<int, uint*, void> _glDeleteBuffers;
        private readonly delegate* unmanaged<uint, void> _glDeleteVertexArrays;
        private readonly delegate* unmanaged<uint, void> _glDeleteProgram;

        public GLWrapper()
        {
            _glDeleteBuffers = (delegate* unmanaged<int, uint*, void>)SDL.GLGetProcAddress("glDeleteBuffers");
            _glDeleteVertexArrays = (delegate* unmanaged<uint, void>)SDL.GLGetProcAddress("glDeleteVertexArrays");
            _glDeleteProgram = (delegate* unmanaged<uint, void>)SDL.GLGetProcAddress("glDeleteProgram");
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
    }

    private readonly IntPtr _glContext;
    private readonly GL _gl;
    private readonly GLWrapper _glWrapper;

#if DEBUG
    private uint _vao;
    private uint _vbo;
    private uint _ebo;
    private uint _program;
#endif

    internal RenderEngineGL() : base(GraphicApi.OpenGL)
    {
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

    internal override void BeforeDraw()
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    internal override void AfterDraw()
    {
        SDL.GLSwapWindow(_windowPtr);
    }

#if DEBUG
    internal unsafe override void DrawTestRect()
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

    protected override void OnDisposeOverride()
    {
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

    protected override void OnViewportSizeChangedOverride(Vector2Int size)
    {
        _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
    }

    private void Initialize()
    {
#if DEBUG
        InitializeDebug();
#endif
    }

#if DEBUG
    private unsafe void InitializeDebug()
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

        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);
        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.BufferData(
            GLEnum.ArrayBuffer, 
            (nuint)(vertices.Length * sizeof(float)), 
            vertices, 
            GLEnum.StaticDraw
        );

        _ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        _gl.BufferData(
            GLEnum.ElementArrayBuffer, 
            (nuint)(indices.Length * sizeof(uint)), 
            indices, 
            GLEnum.StaticDraw
        );

        var vertexShader = _gl.CreateShader(ShaderType.VertexShader);
        _gl.ShaderSource(vertexShader, ShaderSources.VertexShaderSourceGL);
        _gl.CompileShader(vertexShader);

        var infoLog = _gl.GetShaderInfoLog(vertexShader);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Log.Error($"顶点着色器编译失败: {infoLog}");
        }

        var fragmentShader = _gl.CreateShader(ShaderType.FragmentShader);
        _gl.ShaderSource(fragmentShader, ShaderSources.FragmentShaderSourceGL);
        _gl.CompileShader(fragmentShader);

        infoLog = _gl.GetShaderInfoLog(fragmentShader);
        if (!string.IsNullOrWhiteSpace(infoLog))
        {
            Log.Error($"片段着色器编译失败: {infoLog}");
        }

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

        _gl.VertexAttribPointer(
            0,
            2,
            GLEnum.Float,
            false,
            2 * sizeof(float),
            null
        );
        _gl.EnableVertexAttribArray(0);

        _gl.BindVertexArray(0);
    }
#endif
}

#endif