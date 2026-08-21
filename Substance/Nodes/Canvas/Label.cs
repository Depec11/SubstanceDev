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
    public bool IsInScene { get; set; } = false;

    private readonly Texture _texture = new();

    private bool _isDirty = true;

    public Label() : base()
    {
        Font = new Font(this);

        Font.SizeChanged += OnFontSizeChanged;
        Font.ForegroundColorChanged += OnFontColorChanged;
        Font.BackgroundColorChanged += OnBackgroundColorChanged;
    }

    protected override void OnRenderingOverride(double deltaTime)
    {
        if (_isDirty)
        {
            _isDirty = false;
            UpdateTexture();
        }

        DrawString(_texture.Tid, IsInScene ? Viewport.Current.GetMvp(Matrix) : Viewport.Current.GetSvp(Matrix));

    }

    protected virtual void OnTextChanged(PropertyChangedArgs<string> args)
    {
        _isDirty = true;

        TextChanged.Invoke(args);
    }

    private void OnFontSizeChanged(PropertyChangedArgs<uint> args)
    {
        _isDirty = true;
    }

    private void OnFontColorChanged(PropertyChangedArgs<Color> args)
    {
        _isDirty = true;
    }

    private void OnBackgroundColorChanged(PropertyChangedArgs<Color> args)
    {
        _isDirty = true;
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

        Font.SizeChanged -= OnFontSizeChanged;
        Font.ForegroundColorChanged -= OnFontColorChanged;
        Font.BackgroundColorChanged -= OnBackgroundColorChanged;

        _texture.Dispose();

        Font.Dispose();
    }
}
