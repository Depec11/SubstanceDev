using Substance.Core;
using Substance.Graphics;
using Substance.Nodes;

namespace Substance.Components;

public class Font : ComponentBase<Node>
{
    public event Action<PropertyChangedArgs<uint>> SizeChanged = delegate {};
    public event Action<PropertyChangedArgs<Color>> ForegroundColorChanged = delegate {};
    public event Action<PropertyChangedArgs<Color>> BackgroundColorChanged = delegate {};

    public uint Size { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnSizeChanged(new(old, value));
        } } = 16;
    public Color ForegroundColor { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnForegroundColorChanged(new(old, value));
        } } = Color.White;
    public Color BackgroundColor { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnBackgroundColorChanged(new(old, value));
        } } = Color.Transparent;

    public Font(Node owner) : base(owner)
    {
    }

    protected virtual void OnSizeChanged(PropertyChangedArgs<uint> args)
    {
        SizeChanged.Invoke(args);
    }

    protected virtual void OnForegroundColorChanged(PropertyChangedArgs<Color> args)
    {
        ForegroundColorChanged.Invoke(args);
    }

    protected virtual void OnBackgroundColorChanged(PropertyChangedArgs<Color> args)
    {
        BackgroundColorChanged.Invoke(args);
    }
}
