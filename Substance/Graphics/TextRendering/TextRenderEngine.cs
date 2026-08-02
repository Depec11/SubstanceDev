using System.Numerics;
using System.Runtime.CompilerServices;
using Substance.Maths;

namespace Substance.Graphics.TextRendering;

public class TextRenderEngine : IDisposable
{
    private bool _disposed = false;

    ~TextRenderEngine()
    {
        Dispose();
    }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // internal void LoadFont(Uri fontPath) => LoadFontOverride(fontPath);

    // protected virtual void LoadFontOverride(Uri fontPath) {}

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MesasureString(string text, uint fontSize, ref Vector2<int> startPosition, ref Vector2<int> endPosition) => MeasureStringOverride(text, fontSize, ref startPosition, ref endPosition);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RenderString(string text, uint fontSize, Color foregroundColor, Color backgroundColor, ref byte[] texture, ref Vector2<int> size) => RenderStringOverride(text, fontSize, foregroundColor, backgroundColor, ref texture, ref size);

    protected virtual void MeasureStringOverride(string text, uint fontSize, ref Vector2<int> startPosition, ref Vector2<int> endPosition) {}

    protected virtual void RenderStringOverride(string text, uint fontSize, Color foregroundColor, Color backgroundColor, ref byte[] texture, ref Vector2<int> size) {}

    protected virtual void OnDisposeOverride() {}

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        OnDisposeOverride();

        GC.SuppressFinalize(this);
    }
}