using System;

namespace Microsoft.Isam.Esent.Interop
{
	public abstract class EsentResource : IDisposable
	{
		~EsentResource()
		{
			this.Dispose(false);
		}

		protected bool HasResource
		{
			get
			{
				return this.hasResource;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (this.hasResource)
				{
					this.ReleaseResource();
				}
				this.isDisposed = true;
				return;
			}
			bool flag = this.hasResource;
		}

		protected void CheckObjectIsNotDisposed()
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException("EsentResource");
			}
		}

		protected void ResourceWasAllocated()
		{
			this.CheckObjectIsNotDisposed();
			this.hasResource = true;
		}

		protected void ResourceWasReleased()
		{
			this.CheckObjectIsNotDisposed();
			this.hasResource = false;
		}

		protected abstract void ReleaseResource();

		private bool hasResource;

		private bool isDisposed;
	}
}
