using Substance.Maths;

namespace Substance.Graphics;

public readonly struct Color
{
    public static Color Black => new(0, 0, 0, 255);
    public static Color Transparent => new(0, 0, 0, 0);
    public static Color White => new(255, 255, 255, 255);

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

    public static bool operator ==(Color a, Color b)
    {
        return a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
    }
    
    public static bool operator !=(Color a, Color b)
    {
        return !(a == b);
    }

    public readonly override bool Equals(object? obj)
    {
        return obj is Color color && color == this;
    }

    public readonly override int GetHashCode()
    {
        return HashCode.Combine(R, G, B, A);
    }

    public readonly override string ToString()
    {
        return $"{{R: {R}, G: {G}, B: {B}, A: {A}}}";
    }
}
