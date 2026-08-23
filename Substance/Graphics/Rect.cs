using Substance.Maths;

namespace Substance.Graphics;

public struct Rect
{
    public float MinX = 0.0f;
    public float MinY = 0.0f;
    public float MaxX = 0.0f;
    public float MaxY = 0.0f;

    public Rect()
    {
    }

    public Rect(float minX, float minY, float maxX, float maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }

    public readonly bool Contains(in Vector2<float> point)
    {
        return point.X >= MinX && point.X <= MaxX && point.Y >= MinY && point.Y <= MaxY;
    }
}