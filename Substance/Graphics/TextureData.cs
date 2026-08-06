namespace Substance.Graphics;

public record TextureData(int Width, int Height, byte[] Data)
{
    public static readonly TextureData Empty = new(0, 0, []);
}