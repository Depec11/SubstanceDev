using Substance.Windows.Graphics;
using Substance.Graphics;

namespace Substance.Windows;

public class WindowsApplication : Application
{
    public WindowsApplication(WindowOptions? options = null) : base((api) =>
    {
        return api switch
        {
            GraphicApi.OpenGL => new RenderEngineGL(),
            _ => new RenderEngineGL()
        };
    }, options) {}
}
