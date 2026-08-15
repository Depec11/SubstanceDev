using Substance.Maths;

namespace Substance.Core.Events;

public class ViewportSizeChangedArgs : EventArgs
{
    public Vector2<float> OldSize { get; }
    public Vector2<float> NewSize { get; }

    public ViewportSizeChangedArgs(Vector2<float> oldSize, Vector2<float> newSize) : base(EventType.ViewportSizeChanged)
    {
        OldSize = oldSize;
        NewSize = newSize;
    }
}
