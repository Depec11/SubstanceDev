using System.Numerics;

namespace Substance.Maths;

[Obsolete("舒勇INumber<T>接口替代")]
internal static class Scaler<T> where T : struct, INumber<T>
{
    public static readonly T One;
    public static readonly T Zero;

    static Scaler()
    {
        if (typeof(T) == typeof(float))
        {
            One = (T)(object)1.0f;
            Zero = (T)(object)0.0f;
        }
        else if (typeof(T) == typeof(int))
        {
            One = (T)(object)1;
            Zero = (T)(object)0;
        }
        else
        {
            throw new Exception("使用了不支持的类型");
        }
    }

    internal static TypeName Divide<TypeName>(TypeName left, TypeName right) where TypeName : struct, INumber<TypeName>
    {
        if (typeof(TypeName) == typeof(float))
        {
            return (TypeName)(object)((float)(object)left / (float)(object)right);
        }
        else if (typeof(TypeName) == typeof(int))
        {
            return (TypeName)(object)((int)(object)left / (int)(object)right);
        }

        throw new Exception("使用了不支持的类型进行除法");
    }
}