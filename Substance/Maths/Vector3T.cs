global using Vector3Int = Substance.Maths.Vector3T<int>;
global using Vector3 = Substance.Maths.Vector3T<float>;

using System.Numerics;

namespace Substance.Maths;

public struct Vector3T<T> where T : struct, INumber<T>
{
    public T X = default;
    public T Y = default;
    public T Z = default;

    public Vector3T() {}

    public Vector3T(T value)
    {
        X = value;
        Y = value;
        Z = value;
    }

    public Vector3T(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Vector3T<T> One => new(T.One);
    public static Vector3T<T> Zero => new(T.Zero);

    public static bool operator ==(Vector3T<T> a, Vector3T<T> b)
    {
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }

    public static bool operator !=(Vector3T<T> a, Vector3T<T> b)
    {
        return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
    }

    public static Vector3T<T> operator /(Vector3T<T> v, T f)
    {
        return new(v.X / f, v.Y / f, v.Z / f);
    }

    public readonly bool Equals(Vector3T<T> other)
    {
        return X == other.X && Y == other.Y && Z == other.Z;
    }

    public override readonly bool Equals(object? obj)
    {
        return obj is Vector3T<T> vector && Equals(vector);
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