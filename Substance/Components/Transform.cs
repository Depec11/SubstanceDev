using System.Runtime.CompilerServices;
using Substance.Core;
using Substance.Maths;
using Substance.Nodes;

namespace Substance.Components;

public class Transform : ComponentBase<Node>
{
    public event Action<PropertyChangedArgs<Vector2<float>>> PositionChanged = delegate { };
    public event Action<PropertyChangedArgs<Vector2<float>>> ScaleChanged = delegate { };
    public event Action<PropertyChangedArgs<float>> RotationChanged = delegate { };
    public event Action<PropertyChangedArgs<Vector2<float>>> ActualPositionChanged = delegate { };
    public event Action<PropertyChangedArgs<Vector2<float>>> ActualScaleChanged = delegate { };
    public event Action<PropertyChangedArgs<float>> ActualRotationChanged = delegate { };
    public event Action<PropertyChangedArgs<Vector2<float>>> PivotChanged = delegate { };

    public Vector2<float> Position { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnPositionChanged(new PropertyChangedArgs<Vector2<float>>(old, value));
        } } = Vector2<float>.Zero;
    public Vector2<float> Scale { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnScaleChanged(new PropertyChangedArgs<Vector2<float>>(old, value));
        } } = Vector2<float>.One;
    public float Rotation { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnRotationChanged(new PropertyChangedArgs<float>(old, value));
        } } = 0.0f;
    public Vector2<float> ActualPosition { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnActualPositionChanged(new PropertyChangedArgs<Vector2<float>>(old, value));
        } } = Vector2<float>.Zero;
    public Vector2<float> ActualScale { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnActualScaleChanged(new PropertyChangedArgs<Vector2<float>>(old, value));
        } } = Vector2<float>.One;
    public float ActualRotation { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnActualRotationChanged(new PropertyChangedArgs<float>(old, value));
        } } = 0.0f;
    public Vector2<float> Pivot { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnPivotChanged(new PropertyChangedArgs<Vector2<float>>(old, value));
        } } = Vector2<float>.Zero;

    private Transform? _parent;

    public Transform(Node owner) : base(owner)
    {
        if (owner.Parent is not Node node)
        {
            return;
        }
        SetParent(node.Transform);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3x2 GetMatrix(Vector2<float> size) => Matrix3x2.Create(ActualPosition, ActualScale, ActualRotation, size);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Matrix3x2 GetMatrix() => GetMatrix(Vector2<float>.One);

    internal void SetParent(Transform? parent)
    {
        if (_parent == parent)
        {
            return;
        }

        _parent = parent;

        if (_parent is null)
        {
            Position = ActualPosition;
            Scale = ActualScale;
            Rotation = ActualRotation;
        }
        else
        {
            Position = ActualPosition - _parent.Position;
            Scale = ActualScale / _parent.ActualScale;
            Rotation = ActualRotation - _parent.ActualRotation;
        }
    }

    private void OnPositionChanged(PropertyChangedArgs<Vector2<float>> args)
    {
        UpdateActualPosition();
        PositionChanged.Invoke(args);
    }

    private void OnScaleChanged(PropertyChangedArgs<Vector2<float>> args)
    {
        UpdateActualScale();
        ScaleChanged.Invoke(args);
    }

    private void OnRotationChanged(PropertyChangedArgs<float> args)
    {
        UpdateActualRotation();
        RotationChanged.Invoke(args);
    }

    private void OnActualPositionChanged(PropertyChangedArgs<Vector2<float>> args)
    {
        ActualPositionChanged.Invoke(args);
    }

    private void OnActualScaleChanged(PropertyChangedArgs<Vector2<float>> args)
    {
        ActualScaleChanged.Invoke(args);
    }

    private void OnActualRotationChanged(PropertyChangedArgs<float> args)
    {
        ActualRotationChanged.Invoke(args);
    }

    private void OnPivotChanged(PropertyChangedArgs<Vector2<float>> args)
    {
        PivotChanged.Invoke(args);
    }

    private void UpdateActualPosition()
    {
        if (_parent is null)
        {
            ActualPosition = Position;
        }
        else
        {
            ActualPosition = _parent.ActualPosition + Position;
        }
    }

    private void UpdateActualScale()
    {
        if (_parent is null)
        {
            ActualScale = Scale;
        }
        else
        {
            ActualScale = _parent.ActualScale * Scale;
        }
    }

    private void UpdateActualRotation()
    {
        if (_parent is null)
        {
            ActualRotation = Rotation;
        }
        else
        {
            ActualRotation = _parent.ActualRotation + Rotation;
        }
    }
}
