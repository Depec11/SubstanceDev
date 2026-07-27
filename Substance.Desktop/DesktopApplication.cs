using Substance.Desktop.Graphics;
using Substance.Graphics;

namespace Substance.Desktop;

public class DesktopApplication : Application
{
    public DesktopApplication(WindowOptions? options = null) : base((api) =>
    {
        return api switch
        {
            GraphicApi.OpenGL => new RenderEngineGL(),
            _ => new RenderEngineGL()
        };
    }, options) {}
}
