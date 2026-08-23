using Substance.Maths;

namespace Substance.Inputs.Mice;

public class MouseButtonDownEvent : InputEvent
{
    public MouseButtonType ButtonType { get; }

    public Vector2<float> Position { get; }

    public MouseButtonDownEvent(MouseButtonType buttonType, Vector2<float> position) : base(EventType.MouseButtonDown)
    {
        ButtonType = buttonType;
        Position = position;
    }
}
