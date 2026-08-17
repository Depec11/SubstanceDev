using System.Runtime.CompilerServices;
using Substance.Components;
using Substance.Core;
using Substance.Graphics;
using Substance.Maths;

namespace Substance.Nodes;

public class Node : NodeBase
{
    public Transform Transform { get; }
    protected Matrix3x2 Matrix => _matrix;

    private Matrix3x2 _matrix;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void DrawTexture(uint texture, in Matrix3x2 transform, in Vector3<float> modulate) => RenderingServer.Current.DrawTexture(texture, transform, modulate);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void MeasureString(string text, uint fontSize, ref Vector2<int> size) => RenderingServer.Current.MeasureString(text, fontSize, ref size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void RenderString(string text, uint texture, uint fontSize, Color foregroundColor, Color backgroundColor, ref Vector2<int> size) => RenderingServer.Current.RenderString(text, texture, fontSize, foregroundColor, backgroundColor, ref size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected static void DrawString(uint texture, in Matrix3x2 transform) => RenderingServer.Current.DrawString(texture, transform);

    public Node() : base()
    {
        Transform = new Transform(this);

        Transform.ActualPositionChanged += OnTransformPositionChangedOverride;
        Transform.ActualScaleChanged += OnTransformScaleChangedOverride;
        Transform.ActualRotationChanged += OnTransformRotationChangedOverride;
        Transform.PivotChanged += OnTransformPivotChangedOverride;
        
        OnTransformChanged();
    }

    protected virtual void OnTransformChanged()
    {
        UpdateMatrix();
    }

    protected void UpdateMatrix()
    {
        _matrix = Transform.GetMatrix();
    }

    protected void UpdateMatrix(Vector2<float> size)
    {
        _matrix = Transform.GetMatrix(size);
    }

    protected void UpdateMatrix(Vector2<int> size)
    {
        _matrix = Transform.GetMatrix(new Vector2<float>(size.X, size.Y));
    }

    protected virtual void OnTransformPositionChangedOverride(PropertyChangedArgs<Vector2<float>> args)
    {
        OnTransformChanged();
    }

    protected virtual void OnTransformScaleChangedOverride(PropertyChangedArgs<Vector2<float>> args)
    {
        OnTransformChanged();
    }

    protected virtual void OnTransformRotationChangedOverride(PropertyChangedArgs<float> args)
    {
        OnTransformChanged();
    }

    protected virtual void OnTransformPivotChangedOverride(PropertyChangedArgs<Vector2<float>> args)
    {
        OnTransformChanged();
    }

    protected override void OnDisposeOverride()
    {
        base.OnDisposeOverride();
        Transform.ActualPositionChanged -= OnTransformPositionChangedOverride;
        Transform.ActualScaleChanged -= OnTransformScaleChangedOverride;
        Transform.ActualRotationChanged -= OnTransformRotationChangedOverride;
        Transform.PivotChanged -= OnTransformPivotChangedOverride;

        Transform.Dispose();
    }
}