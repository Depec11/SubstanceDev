namespace Substance.Maths;

[Obsolete($"使用{nameof(System.Numerics.Matrix4x4)}")]
public struct Matrix3x3
{
    public static Matrix3x3 Identity => new(1, 0, 0, 0, 1, 0, 0, 0, 1);

    public float M11 = 0;
    public float M12 = 0;
    public float M13 = 0;
    public float M21 = 0;
    public float M22 = 0;
    public float M23 = 0;
    public float M31 = 0;
    public float M32 = 0;
    public float M33 = 0;

    public Matrix3x3() {}

    public Matrix3x3(float value)
    {
        M11 = M12 = M13 = M21 = M22 = M23 = M31 = M32 = M33 = value;
    }

    public Matrix3x3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M21 = m21;
        M22 = m22;
        M23 = m23;
        M31 = m31;
        M32 = m32;
        M33 = m33;
    }

    public static Matrix3x3 CreateScale(Vector2<float> scale) => new(scale.X, 0, 0, 0, scale.Y, 0, 0, 0, 1);

    public static Matrix3x3 CreateTranslation(Vector2<float> position) => new(1, 0, 0, 0, 1, 0, position.X, position.Y, 1);

    public static Matrix3x3 CreateRotation(float rotation)
    {
        var angle = rotation * MathF.PI / 180.0f;
        var cos = MathF.Cos(angle);
        var sin = MathF.Sin(angle);
        return new(cos, -sin, 0, sin, cos, 0, 0, 0, 1);
    }

    public static bool operator ==(Matrix3x3 a, Matrix3x3 b)
    {
        return a.M11 == b.M11 && a.M12 == b.M12 && a.M13 == b.M13 &&
               a.M21 == b.M21 && a.M22 == b.M22 && a.M23 == b.M23 &&
               a.M31 == b.M31 && a.M32 == b.M32 && a.M33 == b.M33;
    }

    public static bool operator !=(Matrix3x3 a, Matrix3x3 b)
    {
        return !(a == b);
    }

    public override readonly bool Equals(object? other)
    {
        return other is Matrix3x3 matrix && matrix == this;
    }

    public override readonly int GetHashCode()
    {
        var r1 = HashCode.Combine(M11, M12, M13);
        var r2 = HashCode.Combine(M21, M22, M23);
        var r3 = HashCode.Combine(M31, M32, M33);
        return HashCode.Combine(r1, r2, r3);
    }

    public static Matrix3x3 operator *(Matrix3x3 a, Matrix3x3 b)
    {
        return new Matrix3x3(
            a.M11 * b.M11 + a.M12 * b.M21 + a.M13 * b.M31,
            a.M11 * b.M12 + a.M12 * b.M22 + a.M13 * b.M32,
            a.M11 * b.M13 + a.M12 * b.M23 + a.M13 * b.M33,
            a.M21 * b.M11 + a.M22 * b.M21 + a.M23 * b.M31,
            a.M21 * b.M12 + a.M22 * b.M22 + a.M23 * b.M32,
            a.M21 * b.M13 + a.M22 * b.M23 + a.M23 * b.M33,
            a.M31 * b.M11 + a.M32 * b.M21 + a.M33 * b.M31,
            a.M31 * b.M12 + a.M32 * b.M22 + a.M33 * b.M32,
            a.M31 * b.M13 + a.M32 * b.M23 + a.M33 * b.M33
        );
    }
}