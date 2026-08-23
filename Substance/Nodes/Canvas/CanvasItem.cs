using Substance.Core;
using Substance.Graphics;
using Substance.Inputs;
using Substance.Inputs.Mice;
using Substance.Maths;

namespace Substance.Nodes.Canvas;

public class CanvasItem : Node
{
    private static TimeSpan s_clickTimeSpan = TimeSpan.FromMilliseconds(500);

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
    public bool IsInScene { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnIsInSceneChanged(new(old, value));
        } } = false;

    protected CanvasStatus _styleType = CanvasStatus.Normal;

    private Rect _rect;
    // private bool _isFocused = false;
    private bool _isPressed = false;
    private bool _isHovering = false;
    private DateTime _pressedTime;

    public CanvasItem() : base()
    {
    }

    protected override void OnTransformChanged()
    {
        UpdateMatrixAndRect();
    }

    protected override void OnInputOverride(InputEvent inputEvent)
    {
        if (inputEvent is MouseMoveEvent mouseMoveEvent)
        {
            // _isHovering = _rect.Contains(mouseMoveEvent.Position);
            if (_isHovering)
            {
                _isHovering = _rect.Contains(mouseMoveEvent.Position);
                if (!_isHovering)
                {
                    UpdateCanvasStatus();
                    OnMouseExitOverride();
                }
            }
            else
            {
                _isHovering = _rect.Contains(mouseMoveEvent.Position);
                if (_isHovering)
                {
                    UpdateCanvasStatus();
                    OnMouseEnterOverride();
                }
            }
        }
        else if (inputEvent is MouseButtonDownEvent mouseButtonDownEvent)
        {
            if (_rect.Contains(mouseButtonDownEvent.Position))
            {
                _isPressed = true;
                // _isFocused = true;
                _pressedTime = DateTime.Now;

                inputEvent.IsHandled = true;
                UpdateCanvasStatus();
                OnMouseDownOverride(mouseButtonDownEvent.ButtonType);
            }
        }
        else if (inputEvent is MouseButtonUpEvent mouseButtonUpEvent && _isPressed)
        {
            _isPressed = false;
            // _isFocused = false;
            inputEvent.IsHandled = true;

            if (_rect.Contains(mouseButtonUpEvent.Position))
            {
                if (DateTime.Now - _pressedTime < s_clickTimeSpan)
                {
                    UpdateCanvasStatus();
                    OnMouseClickOverride();
                }
                else
                {
                    UpdateCanvasStatus();
                    OnMouseUpOverride(mouseButtonUpEvent.ButtonType);
                }
            }
            
            UpdateCanvasStatus();
        }
    }

    protected virtual void OnSizeChangedOverride(PropertyChangedArgs<Vector2<float>> args)
    {
        SizeChanged.Invoke(args);

        UpdateMatrixAndRect();
    }

    protected virtual void OnIsInSceneChangedOverride(PropertyChangedArgs<bool> args)
    {
        // UpdateMatrixAndRect();
        UpdateRect();
    }

    protected virtual void OnMouseEnterOverride() {}

    protected virtual void OnMouseExitOverride() {}

    protected virtual void OnMouseDownOverride(MouseButtonType button) {}

    protected virtual void OnMouseUpOverride(MouseButtonType button) {}

    protected virtual void OnMouseClickOverride() {}

    protected virtual void OnCanvasStatusChanged(CanvasStatus status) {}

    protected void UpdateMatrixAndRect()
    {
        UpdateMatrix(Size);
        UpdateRect();
    }

    private void OnSizeChanged(PropertyChangedArgs<Vector2<float>> args)
    {
        UpdateMatrixAndRect();

        OnSizeChangedOverride(args);

        SizeChanged.Invoke(args);
    }

    private void OnIsInSceneChanged(PropertyChangedArgs<bool> args)
    {
        OnIsInSceneChangedOverride(args);
        // UpdateMatrixAndRect();
        UpdateRect();
    }

    private void UpdateRect()
    {
        // var actualPosition = Transform.ActualPosition;
        // if (IsInScene)
        // {
        //     actualPosition = Viewport.Current.Size / 2.0f;
        // }
        // var position = actualPosition - Transform.Origin * Size * Transform.ActualScale;
        var position = Transform.ActualPosition - Transform.Origin * Size * Transform.ActualScale;
        _rect = new(position.X, position.Y, position.X + Size.X * Transform.ActualScale.X, position.Y + Size.Y * Transform.ActualScale.Y);
    }

    private void UpdateCanvasStatus()
    {
        var oldStyle = _styleType;

        if (_isPressed)
        {
            _styleType = CanvasStatus.Pressed;
        }
        else if (_isHovering)
        {
            _styleType = CanvasStatus.Hovering;
        }
        else
        {
            _styleType = CanvasStatus.Normal;
        }

        if (oldStyle != _styleType)
        {
            OnCanvasStatusChanged(_styleType);
        }
    }

    public enum CanvasStatus
    {
        Normal,
        Pressed,
        Hovering,
    }
}