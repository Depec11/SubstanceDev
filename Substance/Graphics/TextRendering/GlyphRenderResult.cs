namespace Substance.Graphics.TextRendering;

public record GlyphRenderResult(byte[] Data, uint Width, uint Height, int Left, int Top, int AdvanceX)
{
    public static readonly GlyphRenderResult Empty = new([], 0, 0, 0, 0, 0);
}