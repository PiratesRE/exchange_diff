using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public abstract class DisposableBase : IDisposableImpl, IDisposable
	{
		public DisposableBase()
		{
			this.disposableImpl = new DisposableImpl<DisposableBase>(this);
		}

		~DisposableBase()
		{
			this.disposableImpl.FinalizeImpl(this);
		}

		public bool IsDisposed
		{
			get
			{
				return this.disposableImpl.IsDisposed;
			}
		}

		public bool IsDisposing
		{
			get
			{
				return this.disposableImpl.IsDisposing;
			}
		}

		public void SuppressDisposeTracker()
		{
			this.disposableImpl.SuppressTracking();
		}

		public void Dispose()
		{
			this.disposableImpl.DisposeImpl(this);
		}

		protected void CheckDisposed()
		{
			this.disposableImpl.CheckDisposed(this);
		}

		protected abstract DisposeTracker InternalGetDisposeTracker();

		protected abstract void InternalDispose(bool calledFromDispose);

		DisposeTracker IDisposableImpl.InternalGetDisposeTracker()
		{
			return this.InternalGetDisposeTracker();
		}

		void IDisposableImpl.InternalDispose(bool calledFromDispose)
		{
			this.InternalDispose(calledFromDispose);
		}

		private DisposableImpl<DisposableBase> disposableImpl;
	}
}
