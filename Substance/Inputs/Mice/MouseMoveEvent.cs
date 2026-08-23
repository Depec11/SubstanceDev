using Substance.Maths;

namespace Substance.Inputs.Mice;

public class MouseMoveEvent : InputEvent
{
    public Vector2<float> Position { get; }
    public Vector2<float> Delta { get; }

    public MouseMoveEvent(Vector2<float> position, Vector2<float> delta) : base(EventType.MouseMove)
    {
        Position = position;
        Delta = delta;
    }
}
