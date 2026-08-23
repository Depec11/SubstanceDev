using Substance.Components;
using Substance.Core;
using Substance.Graphics;
using Substance.Maths;
using Substance.Styles;

namespace Substance.Nodes.Canvas;

public class Button : CanvasItem
{
    public event Action<Button> Clicked = delegate { };

    public Color BackgroundColor { get; set; } = new Color(1.0f, 0.5f, 0.25f);
    public string Text { get => _label.Text; set => _label.Text = value; }
    public uint FontSize { get => _label.Font.Size; set => _label.Font.Size = value; }
    public CanvasTheme Theme { get; set; } = CanvasTheme.Default;

    private readonly Label _label;
    private readonly Texture _texture;

    public Button() : base()
    {
        _label = new Label
        {
            Transform =
            {
                Origin = new Vector2<float>(0.5f),
            },
            IsInScene = IsInScene,
        };

        UpdateLabelTransform();

        _texture = new Texture(new Uri("assets://Substance/Assets/Pixel.png"));

        OnCanvasStatusChanged(CanvasStatus.Normal);
    }

    protected override void OnRenderingOverride(double deltaTime)
    {
        base.OnRenderingOverride(deltaTime);
    
        var textureMatrix = IsInScene ? Viewport.Current.GetMvp(Matrix) : Viewport.Current.GetSvp(Matrix);
        DrawTexture(_texture.Tid, textureMatrix, BackgroundColor.Vector3);
        _label.OnRendering(deltaTime);
    }

    protected override void OnTransformChanged()
    {
        base.OnTransformChanged();

        UpdateLabelTransform();
    }

    protected override void OnSizeChangedOverride(PropertyChangedArgs<Vector2<float>> args)
    {
        base.OnSizeChangedOverride(args);

        UpdateLabelTransform();
    }

    protected override void OnIsInSceneChangedOverride(PropertyChangedArgs<bool> args)
    {
        base.OnIsInSceneChangedOverride(args);

        _label.IsInScene = args.NewValue;
    }

    protected override void OnMouseClickOverride()
    {
        Clicked.Invoke(this);
    }

    protected override void OnCanvasStatusChanged(CanvasStatus status)
    {
        base.OnCanvasStatusChanged(status);

        switch (status)
        {
            case CanvasStatus.Normal:
                BackgroundColor = Theme.Primary.Normal;
                _label.Modulate = Theme.Secondary.Normal;
                break;
            case CanvasStatus.Pressed:
                BackgroundColor = Theme.Primary.Pressed;
                _label.Modulate = Theme.Secondary.Pressed;
                break;
            case CanvasStatus.Hovering:
                BackgroundColor = Theme.Primary.Hovering;
                _label.Modulate = Theme.Secondary.Hovering;
                break;
        }
    }

    private void UpdateLabelTransform()
    {
        if (_label is null)
        {
            return;
        }

        _label.Transform.ActualPosition = Transform.ActualPosition - Size * Transform.Origin * Transform.ActualScale
             + Size * _label.Transform.Origin * _label.Transform.ActualScale;
        _label.Transform.ActualScale = Transform.ActualScale;
        _label.Transform.ActualRotation = Transform.ActualRotation;
    }
}
