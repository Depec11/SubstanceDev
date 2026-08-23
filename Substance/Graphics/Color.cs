using Substance.Maths;

namespace Substance.Graphics;

public readonly struct Color
{
    public static Color Black => new(0, 0, 0, 255);
    public static Color CornflowerBlue => new(100, 149, 237, 255);
    public static Color LightBlue => new(135, 206, 250, 255);
    public static Color RoyalBlue => new(20, 105, 255, 255);
    public static Color SteelBlue => new(70, 130, 180, 255);
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

    public Color(float r, float g, float b, float a = 1.0f)
    {
        R = (byte)(r * 255.0f);
        G = (byte)(g * 255.0f);
        B = (byte)(b * 255.0f);
        A = (byte)(a * 255.0f);

        Vector3 = new(r, g, b);
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
