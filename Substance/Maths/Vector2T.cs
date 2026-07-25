global using Vector2Int = Substance.Maths.Vector2T<int>;
global using Vector2 = Substance.Maths.Vector2T<int>;

using System.Numerics;

namespace Substance.Maths;

public struct Vector2T<T> where T : struct, INumber<T>
{
    public T X = default;
    public T Y = default;

    public Vector2T() {}

    public Vector2T(T value)
    {
        X = value;
        Y = value;
    }

    public Vector2T(T x, T y)
    {
        X = x;
        Y = y;
    }

    public static Vector2T<T> One => new(T.One);
    public static Vector2T<T> Zero => new(T.Zero);

    public static bool operator ==(Vector2T<T> a, Vector2T<T> b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(Vector2T<T> a, Vector2T<T> b)
    {
        return a.X != b.X || a.Y != b.Y;
    }

    public static Vector2T<T> operator /(Vector2T<T> v, T f)
    {
        return new(v.X / f, v.Y / f);
    }

    public readonly bool Equals(Vector2T<T> other)
    {
        return X == other.X && Y == other.Y;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Vector2T<T> vector && Equals(vector);
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