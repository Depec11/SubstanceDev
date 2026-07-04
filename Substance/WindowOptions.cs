using Substance.Graphics;

namespace Substance;

public class WindowOptions
{
    public Vector2Int Size = new(800, 600);
    public string Title = "单质";
    public GraphicApi GraphicApi = GraphicApi.OpenGL;
}