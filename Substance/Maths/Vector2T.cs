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
}