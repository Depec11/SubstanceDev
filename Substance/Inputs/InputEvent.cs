using SDL3;
using Substance.Inputs.Mice;
using Substance.Inputs.Viewports;
using Substance.Maths;

namespace Substance.Inputs;

public abstract class InputEvent
{
    public EventType Type { get; }
    public bool IsHandled { get; set; }

    public static InputEvent? Create(SDL.Event @event)
    {
        return (SDL.EventType)@event.Type switch
        {
            SDL.EventType.MouseMotion => new MouseMoveEvent(
                new Vector2<float>(@event.Motion.X, @event.Motion.Y), 
                new Vector2<float>(@event.Motion.XRel, @event.Motion.YRel)),
            SDL.EventType.MouseButtonUp => new MouseButtonUpEvent(
                GetMouseButton(@event.Button.Button),
                new Vector2<float>(@event.Button.X, @event.Button.Y)),
            SDL.EventType.MouseButtonDown => new MouseButtonDownEvent(
                GetMouseButton(@event.Button.Button),
                new Vector2<float>(@event.Button.X, @event.Button.Y)),

            SDL.EventType.WindowResized => new ViewportResizedEvent(
                new Vector2<float>(@event.Window.Data1, @event.Window.Data2)),
            
            _ => null,
        };

        static MouseButtonType GetMouseButton(byte button)
        {
            return button switch
            {
                SDL.ButtonLeft => MouseButtonType.Left,
                SDL.ButtonRight => MouseButtonType.Right,
                SDL.ButtonMiddle => MouseButtonType.Middle,
                _ => MouseButtonType.Unknown,
            };
        }
    }

    protected InputEvent(EventType type)
    {
        Type = type;
    }
}