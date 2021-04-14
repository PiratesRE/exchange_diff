using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class IntervalAlignedRecurringTask<T> : RecurringTask<T>
	{
		public IntervalAlignedRecurringTask(Task<T>.TaskCallback callback, T context, TimeSpan alignment) : base(callback, context, alignment)
		{
		}

		public override void Start()
		{
			using (LockManager.Lock(this, LockManager.LockType.Task))
			{
				base.CheckDisposed();
				base.InitialDelay = TimeSpan.FromTicks(base.Interval.Ticks - DateTime.UtcNow.Ticks % base.Interval.Ticks);
				base.Start();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<IntervalAlignedRecurringTask<T>>(this);
		}
	}
}
