namespace Substance.Maths;

public struct Matrix3x2
{
    public readonly float M11 = 0;
    public readonly float M12 = 0;
    public readonly float M21 = 0;
    public readonly float M22 = 0;
    public readonly float M31 = 0;
    public readonly float M32 = 0;

    public static Matrix3x2 Make(Vector2 position, float rotation, Vector2 scale, Vector2 size)
    {
        var sx = size.X * scale.X;
        var sy = size.Y * scale.Y;
        var cos = MathF.Cos(rotation);
        var sin = MathF.Sin(rotation);
        var tx = position.X;
        var ty = position.Y;

        return new Matrix3x2(
            sx * cos,   -sx * sin,
            sy * sin,    sy * cos,
            tx,          ty
        );
    }

    public Matrix3x2(float m11, float m12, float m21, float m22, float m31, float m32)
    {
        M11 = m11;
        M12 = m12;
        M21 = m21;
        M22 = m22;
        M31 = m31;
        M32 = m32;
    }
}