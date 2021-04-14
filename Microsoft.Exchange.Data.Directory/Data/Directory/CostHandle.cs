using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory
{
	internal class CostHandle : DisposeTrackableBase
	{
		public CostHandle(Budget budget, CostType costType, Action<CostHandle> onRelease, string description, TimeSpan preCharge = default(TimeSpan)) : this(budget, onRelease, description, preCharge)
		{
			this.CostType = costType;
		}

		private CostHandle(Budget budget, Action<CostHandle> releaseAction, string description, TimeSpan preCharge)
		{
			if (budget == null)
			{
				throw new ArgumentNullException("budget");
			}
			if (string.IsNullOrEmpty(description))
			{
				ExWatson.SendReport(new ArgumentNullException("cost handle description is null or empty"), ReportOptions.DoNotCollectDumps | ReportOptions.DeepStackTraceHash, null);
			}
			if (preCharge < TimeSpan.Zero)
			{
				throw new ArgumentException("preCharge cannot be a negative timespan", "preCharge");
			}
			this.Budget = budget;
			this.Key = Interlocked.Increment(ref CostHandle.nextKey);
			this.StartTime = TimeProvider.UtcNow - preCharge;
			this.PreCharge = preCharge;
			this.ReleaseAction = releaseAction;
			this.Budget.AddOutstandingAction(this);
			this.DisposedByThread = -1;
			this.DisposedAt = DateTime.MinValue;
			this.Description = description;
			this.MaxLiveTime = Budget.GetMaxActionTime(this.CostType);
		}

		internal TimeSpan MaxLiveTime { get; set; }

		internal long Key { get; private set; }

		internal int DisposedByThread { get; private set; }

		internal DateTime DisposedAt { get; private set; }

		internal Budget Budget { get; private set; }

		internal DateTime StartTime { get; private set; }

		internal CostType CostType { get; private set; }

		internal Action<CostHandle> ReleaseAction { get; private set; }

		internal bool LeakLogged { get; set; }

		internal TimeSpan PreCharge { get; private set; }

		internal string Description { get; private set; }

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CostHandle>(this);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (!this.disposed && isDisposing)
			{
				lock (this.instanceLock)
				{
					if (!this.disposed)
					{
						this.Budget.End(this);
						if (this.ReleaseAction != null)
						{
							this.ReleaseAction(this);
						}
						this.disposed = true;
						this.DisposedByThread = Environment.CurrentManagedThreadId;
						this.DisposedAt = TimeProvider.UtcNow;
					}
				}
			}
		}

		private static long nextKey = long.MinValue;

		private bool disposed;

		private object instanceLock = new object();
	}
}
