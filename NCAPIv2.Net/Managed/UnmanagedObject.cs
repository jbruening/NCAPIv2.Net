using System;
using System.Threading;

namespace NCAPIv2.Managed
{
    /// <summary>
    /// an unmanaged object with a pointer to it's native location
    /// </summary>
    public abstract class UnmanagedObject : DisposableObject
    {
        /// <summary>
        /// the pointer
        /// </summary>
        protected IntPtr _ptr;
        /// <summary>
        /// the pointer
        /// </summary>
        public IntPtr Ptr => _ptr;

        /// <summary>
        /// implicitly cast to an IntPtr
        /// </summary>
        /// <param name="obj"></param>
        public static implicit operator IntPtr(UnmanagedObject obj)
            => obj == null ? IntPtr.Zero : obj._ptr;
    }

    /// <summary>
    /// an object that disposes
    /// </summary>
    public abstract class DisposableObject : IDisposable
    {
        private int _disposed;

        /// <summary>
        /// dispose of the object
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// finalizer
        /// </summary>
        ~DisposableObject()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
            {
                if (disposing)
                    ReleaseManagedResources();
                DisposeObject();
            }
        }

        /// <summary>
        /// only if Dispose is called
        /// </summary>
        protected virtual void ReleaseManagedResources()
        { }

        /// <summary>
        /// called on either finalize or dispose
        /// </summary>
        protected abstract void DisposeObject();
    }
}
