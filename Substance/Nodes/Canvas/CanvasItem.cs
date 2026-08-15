using Substance.Core;
using Substance.Maths;

namespace Substance.Nodes.Canvas;

public class CanvasItem : Node
{
    public event Action<PropertyChangedArgs<Vector2<float>>> SizeChanged = delegate {};

    public Vector2<float> Size { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnSizeChanged(new(old, value));
        } } = Vector2<float>.One;

    public CanvasItem() : base()
    {
        UpdateMatrix(Size);
    }

    protected virtual void OnSizeChanged(PropertyChangedArgs<Vector2<float>> args)
    {
        SizeChanged.Invoke(args);

        UpdateMatrix(args.NewValue);
    }
}