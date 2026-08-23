namespace Substance.Styles;

public class CanvasTheme
{
    public static readonly CanvasTheme Default = new(CanvasStyle.PrimaryDefault, CanvasStyle.SecondaryDefault);

    public CanvasStyle Primary { get; set; }
    public CanvasStyle Secondary { get; set; }

    public CanvasTheme(CanvasStyle primary, CanvasStyle secondary)
    {
        Primary = primary;
        Secondary = secondary;
    }
}