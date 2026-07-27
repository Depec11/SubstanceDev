using System.Numerics;

namespace Substance.Maths;

public struct Vector3<T> where T : struct, INumber<T>
{
    public T X = default;
    public T Y = default;
    public T Z = default;

    public Vector3() {}

    public Vector3(T value)
    {
        X = value;
        Y = value;
        Z = value;
    }

    public Vector3(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vector3<T> One => new(T.One);
    public static Vector3<T> Zero => new(T.Zero);

    public static bool operator ==(Vector3<T> a, Vector3<T> b)
    {
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }

    public static bool operator !=(Vector3<T> a, Vector3<T> b)
    {
        return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
    }

    public static Vector3<T> operator /(Vector3<T> v, T f)
    {
        return new(v.X / f, v.Y / f, v.Z / f);
    }

    public readonly bool Equals(Vector3<T> other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Vector3<T> vector && Equals(vector);
    }

    public override readonly int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public override readonly string ToString()
    {
        return $"({X}, {Y}, {Z})";
    }
}