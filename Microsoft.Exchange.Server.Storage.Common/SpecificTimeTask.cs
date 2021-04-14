using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class SpecificTimeTask<T> : RecurringTask<T>
	{
		public SpecificTimeTask(Task<T>.TaskCallback callback, T context, DateTime triggerUtcTime) : base(callback, context, triggerUtcTime - DateTime.UtcNow, RecurringTask<T>.RunOnce, true)
		{
		}

		public new void Start()
		{
			throw new InvalidOperationException("Can not start a SpecificTimeTask");
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SpecificTimeTask<T>>(this);
		}
	}
}
