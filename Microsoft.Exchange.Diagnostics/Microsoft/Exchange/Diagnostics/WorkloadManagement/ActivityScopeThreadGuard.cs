using System;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal class ActivityScopeThreadGuard : DisposeTrackableBase
	{
		public ActivityScopeThreadGuard(IActivityScope scope)
		{
			if (scope != null)
			{
				this.originalScope = ActivityContext.GetCurrentActivityScope();
				if (!object.ReferenceEquals(this.originalScope, scope))
				{
					ActivityContext.SetThreadScope(scope);
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ActivityScopeThreadGuard>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (!base.IsDisposed && this.originalScope != null)
			{
				ActivityContext.SetThreadScope(this.originalScope);
			}
		}

		private IActivityScope originalScope;
	}
}
