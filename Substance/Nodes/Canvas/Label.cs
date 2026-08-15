using Substance.Components;
using Substance.Core;
using Substance.Graphics;
using Substance.Maths;

namespace Substance.Nodes.Canvas;

public class Label : CanvasItem
{
    public event Action<PropertyChangedArgs<string>> TextChanged = delegate {};

    public string Text { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnTextChanged(new(old, value));
        } } = string.Empty;
    
    public Font Font { get; }

    private readonly Texture _texture = new();

    private bool _isDirty = true;

    public Label() : base()
    {
        Font = new Font(this);

        Font.SizeChanged += args => _isDirty = true;
        Font.ForegroundColorChanged += args => _isDirty = true;
        Font.BackgroundColorChanged += args => _isDirty = true;
    }

    protected override void OnRenderingOverride(double deltaTime)
    {
        if (_isDirty)
        {
            _isDirty = false;
            UpdateTexture();
        }

        DrawString(_texture.Tid, Viewport.Current.GetSvp(Matrix));
    }

    protected virtual void OnTextChanged(PropertyChangedArgs<string> args)
    {
        _isDirty = true;

        TextChanged.Invoke(args);
    }

    private void UpdateTexture()
    {
        var size = Vector2<int>.Zero;
        RenderString(Text, _texture.Tid, Font.Size, Font.ForegroundColor, Font.BackgroundColor, ref size);
        Size = new(size.X, size.Y);
    }

    protected override void OnDisposeOverride()
    {
        base.OnDisposeOverride();

        _texture.Dispose();

        Font.Dispose();
    }
}
