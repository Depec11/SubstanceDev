using Substance.Maths;

namespace Substance.Inputs.Viewports;

public class ViewportResizedEvent : InputEvent
{
    public Vector2<float> NewSize { get; }

    public ViewportResizedEvent(Vector2<float> newSize) : base(EventType.ViewportResized)
    {
        NewSize = newSize;
    }
}
