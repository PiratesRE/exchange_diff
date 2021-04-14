using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal class NullScope : DisposeTrackableBase
	{
		internal NullScope()
		{
			this.localId = SingleContext.Singleton.LocalId;
			this.threadId = Environment.CurrentManagedThreadId;
			SingleContext.Singleton.LocalId = null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<NullScope>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (!base.IsDisposed && disposing)
			{
				ExAssert.RetailAssert(this.threadId == Environment.CurrentManagedThreadId, "ActivityContext.SuppressThreadScope() and NullScope.Dispose() must be called on same thread.");
				SingleContext.Singleton.LocalId = this.localId;
			}
		}

		private readonly int threadId;

		private Guid? localId;
	}
}
