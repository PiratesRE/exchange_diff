using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Clients
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class CachedClient : IDisposeTrackable, IDisposable
	{
		protected CachedClient(IWcfClient client)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.Client = client;
		}

		public virtual bool IsValid
		{
			get
			{
				return this.Client == null || this.Client.IsValid;
			}
		}

		internal bool CanRemoveFromCache
		{
			get
			{
				return this.refCount <= 0;
			}
		}

		internal int ReferenceCount
		{
			get
			{
				return this.refCount;
			}
		}

		private protected IWcfClient Client { protected get; private set; }

		void IDisposable.Dispose()
		{
			Interlocked.Decrement(ref this.refCount);
		}

		void IDisposeTrackable.SuppressDisposeTracker()
		{
			if (this.disposeTracker == null)
			{
				return;
			}
			this.disposeTracker.Suppress();
		}

		public DisposeTracker GetDisposeTracker()
		{
			return this.InternalGetDisposeTracker();
		}

		internal virtual void Cleanup()
		{
			if (this.Client == null)
			{
				return;
			}
			this.Client.Disconnect();
		}

		internal void IncrementReferenceCount()
		{
			Interlocked.Increment(ref this.refCount);
		}

		protected abstract DisposeTracker InternalGetDisposeTracker();

		private readonly DisposeTracker disposeTracker;

		private int refCount;
	}
}
