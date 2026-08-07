using Substance.Android.Graphics;
using Substance.Graphics;

namespace Substance.Android;

public class AndroidApplication : Application
{
    static AndroidApplication()
    {
        Java.Lang.JavaSystem.LoadLibrary("c++_shared");
        Java.Lang.JavaSystem.LoadLibrary("openal");
    }

    public AndroidApplication(WindowOptions? options = null) : base((api) =>
    {
        return api switch
        {
            GraphicApi.OpenGL => new RenderEngineGLES(),
            _ => new RenderEngineGLES()
        };
    }, options)
    {
    }
}