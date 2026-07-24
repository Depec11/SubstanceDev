namespace Substance.Maths;

public struct Vector2Int
{
    public int x = 0;
    public int y = 0;

    public Vector2Int() {}

    public Vector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2Int One => new(1, 1);
    public static Vector2Int Zero => new();

    public static bool operator ==(Vector2Int a, Vector2Int b)
    {
        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(Vector2Int a, Vector2Int b)
    {
        return a.x != b.x || a.y != b.y;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Vector2Int vector && vector == this;
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