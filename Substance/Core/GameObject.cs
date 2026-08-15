// using System.Runtime.CompilerServices;

// namespace Substance.Core;

// public abstract class GameObject : IDisposable
// {
//     private bool _disposed = false;

//     public GameObject()
//     {
//     }

//     ~GameObject()
//     {
//         Dispose();
//     }

//     [MethodImpl(MethodImplOptions.AggressiveInlining)]
//     internal void HandleNotify(NotificationType type) => HandleNotificationOverride(type);

//     protected virtual void HandleNotificationOverride(NotificationType type) {}

//     protected virtual void OnDisposeOverride() {}

//     public void Dispose()
//     {
//         if (_disposed)
//         {
//             return;
//         }

//         _disposed = true;

//         OnDisposeOverride();

//         GC.SuppressFinalize(this);
//     }
// }