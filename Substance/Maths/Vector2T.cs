global using Vector2 = Substance.Maths.Vector2T<float>;
global using Vector2Int = Substance.Maths.Vector2T<int>;

using System.Numerics;

namespace Substance.Maths;

public struct Vector2T<T> where T : struct, INumber<T>
{
    public T X;
    public T Y;

    public Vector2T() {}

    public Vector2T(T x, T y)
    {
        X = x;
        Y = y;
    }

    public static bool operator ==(Vector2T<T> a, Vector2T<T> b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(Vector2T<T> a, Vector2T<T> b)
    {
        return a.X != b.X || a.Y != b.Y;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Vector2T<T> vector && vector == this;
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override readonly string ToString()
    {
        return $"({X}, {Y})";
    }
}