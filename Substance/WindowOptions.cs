using Substance.Graphics;

namespace Substance;

public class WindowOptions
{
    public Vector2Int Size = 
#if ANDROID
    new(0, 0);
#else
    new(800, 600);
#endif
    public string Title = "单质";
    public GraphicApi GraphicApi = GraphicApi.OpenGL;
}