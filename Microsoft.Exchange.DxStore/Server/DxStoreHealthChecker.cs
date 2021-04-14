using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Threading;
using Microsoft.Exchange.DxStore.Common;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.DxStore.Server
{
	public class DxStoreHealthChecker
	{
		public DxStoreHealthChecker(DxStoreInstance instance)
		{
			this.instance = instance;
			this.StoreState = StoreState.Initializing;
			this.instanceClientFactory = new InstanceClientFactory(instance.GroupConfig, null);
			this.WhenMajority = Subject.Synchronize<GroupStatusInfo, GroupStatusInfo>(new Subject<GroupStatusInfo>(), Scheduler.TaskPool);
		}

		public int ConsecutiveMajority { get; set; }

		public int ConsecutiveNoMajority { get; set; }

		public StoreState StoreState { get; set; }

		public ISubject<GroupStatusInfo, GroupStatusInfo> WhenMajority { get; private set; }

		public void Start()
		{
			this.timer = new GuardedTimer(delegate(object unused)
			{
				this.CollectStatus();
			}, null, TimeSpan.Zero, this.instance.GroupConfig.Settings.GroupHealthCheckAggressiveDuration);
		}

		public void Stop()
		{
			this.timer.Dispose(true);
		}

		public bool IsStoreReady()
		{
			return this.instance.IsStartupCompleted && this.ConsecutiveNoMajority < 2 && (this.StoreState & StoreState.Current) == StoreState.Current;
		}

		public GroupStatusInfo GetLastGroupStatusInfo()
		{
			return this.locker.WithReadLock(() => this.groupStatuses.LastOrDefault<GroupStatusInfo>());
		}

		public GroupStatusInfo CollectStatus()
		{
			GroupStatusInfo gsi;
			lock (this.runLock)
			{
				GroupStatusCollector groupStatusCollector = new GroupStatusCollector(this.instance.GroupConfig.Self, this.instanceClientFactory, this.instance.GroupConfig, this.instance.EventLogger, (double)this.instance.GroupConfig.Settings.DefaultHealthCheckRequiredNodePercent);
				groupStatusCollector.Run(this.instance.GroupConfig.Settings.GroupStatusWaitTimeout);
				gsi = groupStatusCollector.GroupStatusInfo;
			}
			this.locker.WithWriteLock(delegate()
			{
				this.DetermineStoreState(gsi);
				this.groupStatuses.Add(gsi);
				if (this.groupStatuses.Count > this.instance.GroupConfig.Settings.MaxEntriesToKeep)
				{
					this.groupStatuses.RemoveAt(0);
				}
			});
			if (gsi.IsMajoritySuccessfulyReplied)
			{
				this.WhenMajority.OnNext(gsi);
			}
			return gsi;
		}

		public void ChangeTimerDuration(TimeSpan duration)
		{
			if (this.timer != null)
			{
				this.timer.Change(duration, duration);
			}
		}

		private void DetermineStoreState(GroupStatusInfo gsi)
		{
			int lag = gsi.Lag;
			if (gsi.LocalInstance == null)
			{
				this.StoreState = StoreState.Initializing;
				return;
			}
			StoreState storeState;
			if (lag > 0)
			{
				bool flag = false;
				GroupStatusInfo groupStatusInfo = this.groupStatuses.LastOrDefault<GroupStatusInfo>();
				if (groupStatusInfo != null && groupStatusInfo.Lag > 0 && groupStatusInfo.LocalInstance != null && groupStatusInfo.LocalInstance.InstanceNumber == gsi.LocalInstance.InstanceNumber)
				{
					flag = true;
				}
				if (!flag)
				{
					if (lag <= this.instance.GroupConfig.Settings.MaximumAllowedInstanceNumberLag)
					{
						storeState = (StoreState.Current | StoreState.CatchingUp);
					}
					else
					{
						storeState = (StoreState.Stale | StoreState.CatchingUp);
					}
				}
				else
				{
					storeState = (StoreState.Stale | StoreState.Struck);
				}
			}
			else
			{
				storeState = StoreState.Current;
			}
			if (gsi.IsMajoritySuccessfulyReplied)
			{
				this.ConsecutiveNoMajority = 0;
				this.ConsecutiveMajority++;
				storeState &= ~StoreState.NoMajority;
			}
			else
			{
				storeState |= StoreState.NoMajority;
				this.ConsecutiveMajority = 0;
				this.ConsecutiveNoMajority++;
			}
			this.StoreState = storeState;
		}

		private readonly List<GroupStatusInfo> groupStatuses = new List<GroupStatusInfo>(10);

		private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

		private readonly object runLock = new object();

		private readonly DxStoreInstance instance;

		private InstanceClientFactory instanceClientFactory;

		private GuardedTimer timer;
	}
}
