using Substance.Core;
using Substance.Graphics;
using Substance.Maths;

namespace Substance.Nodes;

public class Camera : Node
{
    public Camera() : base()
    {    
        Viewport.Current.Position = Transform.ActualPosition;
        Viewport.Current.Rotation = Transform.ActualRotation;
    }

    protected override void OnTransformPositionChangedOverride(PropertyChangedArgs<Vector2<float>> args)
    {
        base.OnTransformPositionChangedOverride(args);

        Viewport.Current.Position = args.NewValue;
    }

    protected override void OnTransformRotationChangedOverride(PropertyChangedArgs<float> args)
    {
        base.OnTransformRotationChangedOverride(args);

        Viewport.Current.Rotation = args.NewValue;
    }

    protected override void OnDisposeOverride()
    {
        base.OnDisposeOverride();

        Viewport.Current.Position = Vector2<float>.Zero;
        Viewport.Current.Rotation = 0;
    }
}