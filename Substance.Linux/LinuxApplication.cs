using Substance.Graphics;
using Substance.Linux.Graphics;

namespace Substance.Linux;

public class LinuxApplication : Application
{
    public LinuxApplication(WindowOptions? options = null) : base((api) =>
    {
        return api switch
        {
            GraphicApi.OpenGL => new RenderEngineGL(),
            _ => new RenderEngineGL()
        };
    }, options) {}
}
