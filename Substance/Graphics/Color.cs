namespace Substance.Graphics;

public struct Color
{
    public readonly byte R, G, B, A;

    public Color(byte r, byte g, byte b, byte? a = null)
    {
        R = r;
        G = g;
        B = b;
        A = a ?? 255;
    }

    public readonly override string ToString()
    {
        return $"{{R: {R}, G: {G}, B: {B}, A: {A}}}";
    }
}
