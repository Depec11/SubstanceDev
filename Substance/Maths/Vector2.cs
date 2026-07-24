namespace Substance.Maths;

public struct Vector2
{
    public float x = 0;
    public float y = 0;

    public Vector2() {}

    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2 One => new(1, 1);
    public static Vector2 Zero => new();

    public static bool operator ==(Vector2 a, Vector2 b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Vector2 a, Vector2 b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Vector2 vector && vector == this;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(x, y);
    }

    public override readonly string ToString()
    {
        return $"({x}, {y})";
    }
}