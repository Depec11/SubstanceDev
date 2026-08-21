using System.Numerics;

namespace Substance.Maths;

// [Obsolete($"使用{nameof(Vector3)}")]
public struct Vector3<T> where T : struct, INumber<T>
{
    public static Vector3<T> One => new(T.One);
    public static Vector3<T> Zero => new(T.Zero);

    public readonly T SquaredLength => X * X + Y * Y + Z * Z;
    public readonly T Length => (T)(object)Math.Sqrt((double)(object)SquaredLength);
    public readonly Vector3<T> Normalized { get
        {
            var length = Length;
            return new(X / length, Y / length, Z / length);
        } }

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

    public void Normalize()
    {
        var length = Length;
        X /= length;
        Y /= length;
        Z /= length;
    }

    public readonly T Dot(Vector3<T> other)
    {
        return X * other.X + Y * other.Y + Z * other.Z;
    }

    public readonly Vector3<T> Cross(Vector3<T> other)
    {
        return new(Y * other.Z - Z * other.Y, Z * other.X - X * other.Z, X * other.Y - Y * other.X);
    }

    public static bool operator ==(Vector3<T> a, Vector3<T> b)
    {
        return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
    }

    public static bool operator !=(Vector3<T> a, Vector3<T> b)
    {
        return a.X != b.X || a.Y != b.Y || a.Z != b.Z;
    }

    public static Vector3<T> operator +(Vector3<T> v)
    {
        return new(v.X, v.Y, v.Z);
    }

    public static Vector3<T> operator -(Vector3<T> v)
    {
        return new(-v.X, -v.Y, -v.Z);
    }

    public static Vector3<T> operator +(Vector3<T> v, T f)
    {
        return new(v.X + f, v.Y + f, v.Z + f);
    }

    public static Vector3<T> operator -(Vector3<T> v, T f)
    {
        return new(v.X - f, v.Y - f, v.Z - f);
    }

    public static Vector3<T> operator *(Vector3<T> v, T f)
    {
        return new(v.X * f, v.Y * f, v.Z * f);
    }

    public static Vector3<T> operator /(Vector3<T> v, T f)
    {
        return new(v.X / f, v.Y / f, v.Z / f);
    }

    public static Vector3<T> operator +(T f, Vector3<T> v)
    {
        return new(v.X + f, v.Y + f, v.Z + f);
    }

    public static Vector3<T> operator -(T f, Vector3<T> v)
    {
        return new(v.X - f, v.Y - f, v.Z - f);
    }

    public static Vector3<T> operator *(T f, Vector3<T> v)
    {
        return new(v.X * f, v.Y * f, v.Z * f);
    }

    public static Vector3<T> operator /(T f, Vector3<T> v)
    {
        return new(v.X / f, v.Y / f, v.Z / f);
    }

    public static Vector3<T> operator +(Vector3<T> v1, Vector3<T> v2)
    {
        return new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
    }

    public static Vector3<T> operator -(Vector3<T> v1, Vector3<T> v2)
    {
        return new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
    }

    public static Vector3<T> operator *(Vector3<T> v1, Vector3<T> v2)
    {
        return new(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
    }

    public static Vector3<T> operator /(Vector3<T> v1, Vector3<T> v2)
    {
        return new(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
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