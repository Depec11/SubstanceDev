using Substance.Maths;

namespace Substance.Graphics;

public readonly struct Color
{
    public static Color White => new(255, 255, 255);

    public readonly Vector3<float> Vector3;

    public readonly byte R, G, B, A;

    public Color(byte r, byte g, byte b, byte? a = null)
    {
        R = r;
        G = g;
        B = b;
        A = a ?? 255;

        Vector3 = new(R / 255.0f, G / 255.0f, B / 255.0f);
    }

    public readonly override string ToString()
    {
        return $"{{R: {R}, G: {G}, B: {B}, A: {A}}}";
    }
}
