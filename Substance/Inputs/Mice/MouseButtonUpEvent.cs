using Substance.Maths;

namespace Substance.Inputs.Mice;

public class MouseButtonUpEvent : InputEvent
{
    public MouseButtonType ButtonType { get; }

    public Vector2<float> Position { get; }

    public MouseButtonUpEvent(MouseButtonType buttonType, Vector2<float> position) : base(EventType.MouseButtonUp)
    {
        ButtonType = buttonType;
        Position = position;
    }
}
