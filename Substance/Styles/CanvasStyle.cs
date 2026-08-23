using Substance.Graphics;

namespace Substance.Styles;

public class CanvasStyle
{
    public static readonly CanvasStyle PrimaryDefault = new(Color.CornflowerBlue, Color.SteelBlue, Color.LightBlue);
    public static readonly CanvasStyle SecondaryDefault = new(Color.White, Color.White, Color.White);

    public Color Normal { get; set; } = Color.White;
    public Color Pressed { get; set; } = Color.White;
    public Color Hovering { get; set; } = Color.White;

    public CanvasStyle()
    {
    }

    public CanvasStyle(Color normal, Color pressed, Color hovering)
    {
        Normal = normal;
        Pressed = pressed;
        Hovering = hovering;
    }
}