using System;
using System.Threading;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal abstract class ReaderWriterLockScopeBase : IDisposable
	{
		protected ReaderWriterLockScopeBase(ReaderWriterLockSlim readerWriterLock)
		{
			this.readerWriterLock = readerWriterLock;
		}

		protected ReaderWriterLockSlim ScopedReaderWriterLock
		{
			get
			{
				return this.readerWriterLock;
			}
		}

		protected bool IsDisposed
		{
			get
			{
				return this.disposed;
			}
		}

		protected abstract void Acquire();

		protected abstract void Release();

		~ReaderWriterLockScopeBase()
		{
			this.Dispose(false);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				this.disposed = true;
				if (disposing)
				{
					this.Release();
				}
			}
		}

		private bool disposed;

		private readonly ReaderWriterLockSlim readerWriterLock;
	}
}
