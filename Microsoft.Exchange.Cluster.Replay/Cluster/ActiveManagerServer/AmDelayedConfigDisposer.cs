using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDelayedConfigDisposer : TimerComponent
	{
		public AmDelayedConfigDisposer() : base(TimeSpan.FromMilliseconds(-1.0), TimeSpan.FromMilliseconds(-1.0), "AmDelayedConfigDisposer")
		{
		}

		protected override void StopInternal()
		{
			base.StopInternal();
			this.CleanupObjects(false);
		}

		internal void AddEntry(AmConfig cfg)
		{
			AmDelayedConfigDisposer.DisposableContainer disposableContainer = new AmDelayedConfigDisposer.DisposableContainer(cfg);
			lock (this.m_locker)
			{
				ExTraceGlobals.AmConfigManagerTracer.TraceDebug<string, string>(0L, "Adding AmConfig for delayed dispose (Queued for Dispose: {0}, Dispose due at: {1})", disposableContainer.TimeRequestSubmitted.ToString(), disposableContainer.DisposeDueAt.ToString());
				this.m_waitingList.Add(disposableContainer);
				if (disposableContainer.DisposeDueAt < this.m_lowestDueTime)
				{
					this.m_lowestDueTime = disposableContainer.DisposeDueAt;
					this.SetupWakeupTime(this.m_lowestDueTime);
				}
			}
		}

		protected override void TimerCallbackInternal()
		{
			this.CleanupObjects(false);
		}

		private void CleanupObjects(bool isForce)
		{
			List<AmDelayedConfigDisposer.DisposableContainer> list = new List<AmDelayedConfigDisposer.DisposableContainer>(5);
			ExTraceGlobals.AmConfigManagerTracer.TraceDebug(0L, "Entering CleanupObjects");
			lock (this.m_locker)
			{
				ExDateTime now = ExDateTime.Now;
				this.m_lowestDueTime = ExDateTime.MaxValue;
				bool flag2 = false;
				foreach (AmDelayedConfigDisposer.DisposableContainer disposableContainer in this.m_waitingList)
				{
					if (object.ReferenceEquals(disposableContainer.Config, AmSystemManager.Instance.Config))
					{
						ExTraceGlobals.AmConfigManagerTracer.TraceDebug(0L, "System manager is still using config.");
						flag2 = true;
					}
					if (isForce || (now > disposableContainer.DisposeDueAt && !flag2))
					{
						list.Add(disposableContainer);
					}
					else if (disposableContainer.DisposeDueAt < this.m_lowestDueTime)
					{
						this.m_lowestDueTime = disposableContainer.DisposeDueAt;
					}
				}
				foreach (AmDelayedConfigDisposer.DisposableContainer item in list)
				{
					this.m_waitingList.Remove(item);
				}
				if (!isForce && flag2)
				{
					if (this.m_lowestDueTime == ExDateTime.MaxValue)
					{
						this.m_lowestDueTime = ExDateTime.Now.AddMinutes(5.0);
					}
					ExTraceGlobals.AmConfigManagerTracer.TraceDebug<string>(0L, "System manager is still using config. It will try to dispose at {0}", this.m_lowestDueTime.ToString());
				}
				this.SetupWakeupTime(this.m_lowestDueTime);
			}
			foreach (AmDelayedConfigDisposer.DisposableContainer disposableContainer2 in list)
			{
				try
				{
					disposableContainer2.Dispose();
				}
				catch (ClusterException arg)
				{
					ExTraceGlobals.AmConfigManagerTracer.TraceDebug<ClusterException>(0L, "AmDelayedConfigDisposer encountered exception {0} while disposing", arg);
				}
			}
			ExTraceGlobals.AmConfigManagerTracer.TraceDebug(0L, "Exiting CleanupObjects");
		}

		private void SetupWakeupTime(ExDateTime lowestTime)
		{
			if (lowestTime != ExDateTime.MaxValue)
			{
				ExDateTime now = ExDateTime.Now;
				TimeSpan dueTime = TimeSpan.Zero;
				if (now < lowestTime)
				{
					dueTime = (lowestTime - now).Add(TimeSpan.FromSeconds(2.0));
				}
				base.ChangeTimer(dueTime, TimeSpan.FromMilliseconds(-1.0));
				return;
			}
			base.ChangeTimer(TimeSpan.FromMilliseconds(-1.0), TimeSpan.FromMilliseconds(-1.0));
		}

		private List<AmDelayedConfigDisposer.DisposableContainer> m_waitingList = new List<AmDelayedConfigDisposer.DisposableContainer>(5);

		private object m_locker = new object();

		private ExDateTime m_lowestDueTime = ExDateTime.MaxValue;

		internal class DisposableContainer : DisposeTrackableBase
		{
			public DisposableContainer(AmConfig cfg)
			{
				this.Config = cfg;
				this.TimeRequestSubmitted = ExDateTime.Now;
				this.DisposeDueAt = this.TimeRequestSubmitted.AddSeconds((double)RegistryParameters.AmConfigObjectDisposeDelayInSec);
			}

			internal ExDateTime TimeRequestSubmitted { get; set; }

			internal ExDateTime DisposeDueAt { get; set; }

			internal AmConfig Config { get; set; }

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<AmDelayedConfigDisposer.DisposableContainer>(this);
			}

			protected override void InternalDispose(bool disposing)
			{
				lock (this)
				{
					if (disposing)
					{
						AmConfig config = this.Config;
						string arg = config.TimeCreated.ToString();
						ExTraceGlobals.AmConfigManagerTracer.TraceDebug<string>(0L, "Disposing AmConfig sub objects (Creation Time: {0})", arg);
						if (config.DbState != null)
						{
							ExTraceGlobals.AmConfigManagerTracer.TraceDebug<string>(0L, "Disposing DbState of AmConfig (Creation Time: {0})", arg);
							this.Config.DbState.Dispose();
						}
						AmDagConfig dagConfig = this.Config.DagConfig;
						if (dagConfig != null && dagConfig.Cluster != null)
						{
							ExTraceGlobals.AmConfigManagerTracer.TraceDebug<string>(0L, "Disposing Cluster of AmConfig (Creation Time: {0})", arg);
							dagConfig.Cluster.Dispose();
						}
						config.IsInternalObjectsDisposed = true;
					}
				}
			}
		}
	}
}
