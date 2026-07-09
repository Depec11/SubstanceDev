#if !ANDROID

using SDL3;
using Silk.NET.OpenGL;
using Substance.Logging;

namespace Substance.Graphics;

public class RenderEngineGL : RenderEngine
{
    private readonly IntPtr _glContext;
    private readonly GL _gl;

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
        
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        // 100, 149, 237, 255
        _gl.ClearColor(0.392f, 0.584f, 0.929f, 1.0f);
        _gl.Viewport(0, 0, (uint)window.Size.X, (uint)window.Size.Y);

        Log.Info($"[{nameof(RenderEngineGL)}] 创建成功");
    }

    protected override void BeforeDrawOverride()
    {
        _gl.Clear(ClearBufferMask.ColorBufferBit);
    }

    protected override void AfterDrawOverride()
    {
        SDL.GLSwapWindow(_windowPtr);
    }

    protected override void OnDisposeOverride()
    {
        SDL.GLDestroyContext(_glContext);

        Log.Info($"[{nameof(RenderEngineGL)}] 销毁成功");
    }

    protected override void OnViewportSizeChangedOverride(Vector2Int size)
    {
        _gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);
    }
}

#endif