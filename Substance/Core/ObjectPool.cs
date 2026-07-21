using System.Diagnostics.CodeAnalysis;

namespace Substance.Core;

public class ObjectPool<T> : IDisposable where T : class, IPoolObject<T>, new()
{
    private readonly Queue<int> _freeIndices = new();
    private readonly HashSet<int> _usedIndices = [];

    private T[] _data;
    private bool disposed = false;

    public ObjectPool(int size = 32)
    {
        _data = new T[size];

        for (int i = 0; i < size; i++)
        {
            _data[i] = new T();
            _freeIndices.Enqueue(i);
        }
    }

    ~ObjectPool()
    {
        Dispose();
    }

    public int Get(out T data)
    {
        if (_freeIndices.Count <= 0)
        {
            Extend();
        }

        var index = _freeIndices.Dequeue();
        _usedIndices.Add(index);

        data = _data[index];
        return index;
    }

    public bool Release(int index)
    {
        if (index < 0 || index >= _data.Length)
        {
            return false;
        }

        if (!_usedIndices.Remove(index))
        {
            return false;
        }

        _freeIndices.Enqueue(index);
        _data[index].Reset();

        return true;
    }

    public T? Get(int index)
    {
        if (index < 0 || index >= _data.Length)
        {
            return null;
        }

        if (!_usedIndices.Contains(index))
        {
            return null;
        }

        return _data[index];
    }

    public bool TryGet(int index, [MaybeNullWhen(false)] out T value)
    {
        value = Get(index);
        return value is not null;
    }

    private void Extend()
    {
        var newData = new T[_data.Length * 2];

        Array.Copy(_data, newData, _data.Length);

        for (var i = _data.Length; i < newData.Length; i++)
        {
            newData[i] = new T();
            _freeIndices.Enqueue(i);
        }

        _data = newData;
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        disposed = true;

        foreach (var data in _data)
        {
            data.Dispose();
        }
        _data = [];
        
        GC.SuppressFinalize(this);
    }
}