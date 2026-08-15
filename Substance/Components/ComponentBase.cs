using Substance.Nodes;

namespace Substance.Components;

public abstract class ComponentBase<T> : IDisposable where T : NodeBase
{
    private static uint s_idCount = 0u;

    public uint Cid { get; } = s_idCount++;
    public T Owner => _owner;
    
    private readonly T _owner;
    private bool _disposed = false;

    protected ComponentBase(T owner)
    {
        _owner = owner;
    }

    ~ComponentBase()
    {
        Dispose();
    }
    
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
