using System.Numerics;

namespace Substance.Maths;

public struct Vector2<T> where T : struct, INumber<T>
{
    public T X = default;
    public T Y = default;

    public Vector2() {}

    public Vector2(T value)
    {
        X = value;
        Y = value;
    }

    public Vector2(T x, T y)
    {
        X = x;
        Y = y;
    }

    public static Vector2<T> One => new(T.One);
    public static Vector2<T> Zero => new(T.Zero);

    public static bool operator ==(Vector2<T> a, Vector2<T> b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(Vector2<T> a, Vector2<T> b)
    {
        return a.X != b.X || a.Y != b.Y;
    }

    public static Vector2<T> operator +(Vector2<T> v, T f)
    {
        return new(v.X + f, v.Y + f);
    }

    public static Vector2<T> operator -(Vector2<T> v, T f)
    {
        return new(v.X - f, v.Y - f);
    }

    public static Vector2<T> operator *(Vector2<T> v, T f)
    {
        return new(v.X * f, v.Y * f);
    }

    public static Vector2<T> operator /(Vector2<T> v, T f)
    {
        return new(v.X / f, v.Y / f);
    }

    public static Vector2<T> operator +(Vector2<T> v1, Vector2<T> v2)
    {
        return new(v1.X + v2.X, v1.Y + v2.Y);
    }

    public static Vector2<T> operator -(Vector2<T> v1, Vector2<T> v2)
    {
        return new(v1.X - v2.X, v1.Y - v2.Y);
    }

    public static Vector2<T> operator *(Vector2<T> v1, Vector2<T> v2)
    {
        return new(v1.X * v2.X, v1.Y * v2.Y);
    }

    public static Vector2<T> operator /(Vector2<T> v1, Vector2<T> v2)
    {
        return new(v1.X / v2.X, v1.Y / v2.Y);
    }

    public readonly bool Equals(Vector2<T> other)
    {
        return X == other.X && Y == other.Y;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Vector2<T> vector && Equals(vector);
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