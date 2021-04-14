using System;
using System.Threading;

namespace Microsoft.Exchange.Data.Internal
{
	internal abstract class RefCountable : IDisposable
	{
		protected bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		protected void ThrowIfDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException("RefCountable");
			}
		}

		protected RefCountable()
		{
			this.refCount = 1;
		}

		public int RefCount
		{
			get
			{
				return this.refCount;
			}
		}

		public void AddRef()
		{
			this.ThrowIfDisposed();
			Interlocked.Increment(ref this.refCount);
		}

		public void Release()
		{
			this.ThrowIfDisposed();
			if (Interlocked.Decrement(ref this.refCount) == 0)
			{
				this.Dispose();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			this.isDisposed = true;
		}

		private int refCount;

		private bool isDisposed;
	}
}
