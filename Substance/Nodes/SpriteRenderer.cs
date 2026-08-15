using Substance.Core;
using Substance.Graphics;

namespace Substance.Nodes;

public class SpriteRenderer : Node
{
    public Texture? Texture { get; set
        {
            if (field == value)
            {
                return;
            }

            var old = field;
            field = value;
            OnTextureChanged(new(old, value));
            old?.Dispose();
        } }
    
    public Color Color { get; set; } = Color.White;

    public SpriteRenderer()
    {
        if (Texture is not null)
        {
            UpdateMatrix(Texture.Size);
        }
    }

    protected virtual void OnTextureChanged(PropertyChangedArgs<Texture?> args)
    {
        if (args.NewValue is not null)
        {
            UpdateMatrix(args.NewValue.Size);
        }
    }

    protected override void OnRenderingOverride(double deltaTime)
    {
        base.OnRenderingOverride(deltaTime);

        if (Texture is null)
        {
            return;
        }

        DrawTexture(Texture.Tid, Viewport.Current.GetSvp(Matrix), Color.Vector3);
    }

    protected override void OnDisposeOverride()
    {
        base.OnDisposeOverride();

        Texture?.Dispose();
    }
}
