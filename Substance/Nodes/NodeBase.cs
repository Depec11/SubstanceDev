using System.Runtime.CompilerServices;
using Substance.Inputs;

namespace Substance.Nodes;

public abstract class NodeBase : IDisposable
{
    private static uint s_idCount = 0u;

    public uint Nid { get; } = s_idCount++;
    public NodeBase? Parent => _parent;
    public IEnumerable<NodeBase> Children => _children;

    private NodeBase? _parent = null;
    private readonly List<NodeBase> _children = [];
    private bool _isInTree = false;
    private bool _disposed = false;

    ~NodeBase()
    {
        Dispose();
    }

    internal void OnEnterTree()
    {
        _isInTree = true;

        OnEnterTreeOverride();

        foreach (var child in _children)
        {
            child.OnEnterTree();
        }
    }

    internal void OnExitTree()
    {
        _isInTree = false;

        foreach (var child in _children)
        {
            child.OnExitTree();

            child.Dispose();
        }

        OnExitTreeOverride();

        Dispose();
    }

    internal void OnUpdate(double deltaTime)
    {
        OnUpdateOverride(deltaTime);

        foreach (var child in _children)
        {
            child.OnUpdate(deltaTime);
        }
    }

    internal void OnRendering(double deltaTime)
    {
        OnRenderingOverride(deltaTime);

        foreach (var child in _children)
        {
            child.OnRendering(deltaTime);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void HandleInput(InputEvent inputEvent)
    {
        OnInputOverride(inputEvent);

        if (inputEvent.IsHandled)
        {
            return;
        }
        
        var siblings = _parent?._children ?? [];
        
        foreach (var sibling in siblings)
        {
            if (sibling == this)
            {
                continue;
            }

            sibling.OnInputOverride(inputEvent);

            if (inputEvent.IsHandled)
            {
                return;
            }
        }

        _parent?.HandleInput(inputEvent);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddChild(NodeBase child)
    {
        child.SetParent(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RemoveChild(NodeBase child)
    {
        child.SetParent(null);
    }

    public void SetParent(NodeBase? parent)
    {
        if (_parent == parent)
        {
            return;
        }

        var wasInTree = _isInTree;

        _parent?._children.Remove(this);
        _parent = parent;
        if (this is Node node)
        {
            node.Transform.SetParent(parent);
        }
        parent?._children.Add(this);

        var nowInTree = parent is not null && parent._isInTree;

        if (!wasInTree && nowInTree)
        {
            OnEnterTree();
        }
        else if (wasInTree && !nowInTree)
        {
            OnExitTree();
        }

        OnParentChangedOverride();
    }

    public void ExitTree()
    {
        if (!_isInTree)
        {
            return;
        }

        SetParent(null);
    }

    public void ClearChildren()
    {
        foreach (var child in _children.ToList())
        {
            RemoveChild(child);
        }
    }

    public NodeBase? GetLastChild()
    {
        return _children.LastOrDefault();
    }

    public NodeBase? GetLastChildIterator()
    {
        var res = GetLastChild();

        while (res is not null)
        {
            var lastChild = res.GetLastChild();

            if (lastChild is null)
            {
                return res;
                
            }
            else
            {
                res = lastChild;
            }
        }

        return null;
    }

    // public void NotifyForward(NotificationType type)
    // {
    //     HandleNotification(type);
    //     foreach (var child in _children)
    //     {
    //         child.NotifyBackward(type);
    //     }
    // }

    // public void NotifyBackward(NotificationType type)
    // {
    //     HandleNotification(type);
    //     Parent?.NotifyForward(type);
    // }

    // [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // internal void HandleNotification(NotificationType type) => HandleNotificationOverride(type);

    // protected virtual void HandleNotificationOverride(NotificationType type) {}

    protected virtual void OnEnterTreeOverride() {}

    protected virtual void OnExitTreeOverride() {}

    protected virtual void OnUpdateOverride(double deltaTime) {}

    protected virtual void OnRenderingOverride(double deltaTime) {}

    protected virtual void OnParentChangedOverride() {}

    protected virtual void OnInputOverride(InputEvent inputEvent) {}

    protected virtual void OnDisposeOverride() {}

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        
        _disposed = true;

        ExitTree();

        OnDisposeOverride();

        GC.SuppressFinalize(this);
    }
}
