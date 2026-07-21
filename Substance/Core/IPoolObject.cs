namespace Substance.Core;

public interface IPoolObject<T>: IDisposable
{
    void Reset();
}