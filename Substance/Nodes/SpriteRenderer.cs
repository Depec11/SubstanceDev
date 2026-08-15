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
        UpdateTransformMatrix();
    }

    protected virtual void OnTextureChanged(PropertyChangedArgs<Texture?> args)
    {
        UpdateTransformMatrix();
    }

    protected override void OnTransformChanged()
    {
        UpdateTransformMatrix();
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

    private void UpdateTransformMatrix()
    {
        if (Texture is null)
        {
            UpdateMatrix();

            return;
        }

        UpdateMatrix(Texture.Size);
    }

    protected override void OnDisposeOverride()
    {
        base.OnDisposeOverride();

        Texture?.Dispose();
    }
}
