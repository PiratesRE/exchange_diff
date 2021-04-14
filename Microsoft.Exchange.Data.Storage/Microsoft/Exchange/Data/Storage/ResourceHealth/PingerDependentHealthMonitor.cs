using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class PingerDependentHealthMonitor : CacheableResourceHealthMonitor
	{
		public static Action<ResourceKey, Guid, PingerDependentHealthMonitor.PingResult> OnExecuteForTest { get; set; }

		public static Action<ResourceKey, int, TimeSpan> OnPingIntervalUpdate { get; set; }

		public static bool IgnorePingProximity { get; set; }

		static PingerDependentHealthMonitor()
		{
			PingerDependentHealthMonitor.ReadPingConfiguration();
		}

		internal static TimeSpan MinimumPingInterval { get; private set; }

		internal static TimeSpan MaximumPingInterval { get; private set; }

		internal virtual DateTime RawLastUpdateUtc
		{
			get
			{
				return this.LastUpdateUtc;
			}
		}

		private static void ReadPingConfiguration()
		{
			TimeSpanAppSettingsEntry timeSpanAppSettingsEntry = new TimeSpanAppSettingsEntry("MinimumPingIntervalInSeconds", TimeSpanUnit.Seconds, PingerDependentHealthMonitor.DefaultMinimumPingInterval, ExTraceGlobals.DatabasePingerTracer);
			TimeSpanAppSettingsEntry timeSpanAppSettingsEntry2 = new TimeSpanAppSettingsEntry("MaximumPingIntervalInSeconds", TimeSpanUnit.Seconds, PingerDependentHealthMonitor.DefaultMaximumPingInterval, ExTraceGlobals.DatabasePingerTracer);
			PingerDependentHealthMonitor.SetPingerIntervals(timeSpanAppSettingsEntry.Value, timeSpanAppSettingsEntry2.Value);
		}

		internal static void SetPingerIntervals(TimeSpan minToUse, TimeSpan maxToUse)
		{
			if (minToUse > TimeSpan.Zero && maxToUse >= minToUse)
			{
				PingerDependentHealthMonitor.MinimumPingInterval = minToUse;
				PingerDependentHealthMonitor.MaximumPingInterval = maxToUse;
				return;
			}
			PingerDependentHealthMonitor.MinimumPingInterval = PingerDependentHealthMonitor.DefaultMinimumPingInterval;
			PingerDependentHealthMonitor.MaximumPingInterval = PingerDependentHealthMonitor.DefaultMaximumPingInterval;
		}

		internal PingerDependentHealthMonitor(ResourceKey key, Guid mdbGuid) : base(key)
		{
			if (mdbGuid == Guid.Empty)
			{
				throw new ArgumentException("[PingerDependentHealthMonitor.ctor] mdbGuid cannot be Guid.Empty");
			}
			this.MdbGuid = mdbGuid;
		}

		public override ResourceLoad GetResourceLoad(WorkloadClassification classification, bool raw = false, object optionalData = null)
		{
			this.PingIfNecessary();
			return base.GetResourceLoad(classification, raw, null);
		}

		internal virtual TimeSpan CurrentPingInterval
		{
			get
			{
				return this.GetPingInterval(this.consecutivePings);
			}
		}

		internal virtual TimeSpan PreviousPingInterval
		{
			get
			{
				return this.GetPingInterval(this.consecutivePings - 1);
			}
		}

		internal virtual TimeSpan GetPingInterval(int consecutivePings)
		{
			TimeSpan timeSpan;
			if (consecutivePings < 0)
			{
				timeSpan = PingerDependentHealthMonitor.MinimumPingInterval;
			}
			else if (consecutivePings > 10)
			{
				timeSpan = PingerDependentHealthMonitor.MaximumPingInterval;
			}
			else
			{
				timeSpan = TimeSpan.FromSeconds(PingerDependentHealthMonitor.MinimumPingInterval.TotalSeconds * Math.Pow(2.0, (double)consecutivePings));
			}
			if (timeSpan > PingerDependentHealthMonitor.MaximumPingInterval)
			{
				timeSpan = PingerDependentHealthMonitor.MaximumPingInterval;
			}
			return timeSpan;
		}

		private void PingIfNecessary()
		{
			if (this.pingCheckedOrScheduled)
			{
				this.DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult.PingAlreadyScheduled);
				return;
			}
			bool flag = true;
			try
			{
				lock (this.instanceLock)
				{
					if (this.pingCheckedOrScheduled)
					{
						this.DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult.PingAlreadyScheduled);
						return;
					}
					this.pingCheckedOrScheduled = true;
				}
				if (this.RawLastUpdateUtc != DateTime.MinValue && TimeProvider.UtcNow - this.RawLastUpdateUtc < this.PreviousPingInterval && !PingerDependentHealthMonitor.IgnorePingProximity)
				{
					this.DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult.TrafficTooClose);
				}
				else if (this.Expired)
				{
					this.DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult.MonitorExpired);
					ExTraceGlobals.DatabasePingerTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[PingerDependentHealthMonitor.Execute] Will not ping on resource {0} because the monitor is marked as expired.", base.Key);
				}
				else
				{
					IMdbSystemMailboxPinger mdbSystemMailboxPinger = PingerCache.Singleton.Get(this.MdbGuid);
					if (!object.ReferenceEquals(mdbSystemMailboxPinger, this.pingerReference))
					{
						lock (this.instanceLock)
						{
							if (!object.ReferenceEquals(mdbSystemMailboxPinger, this.pingerReference))
							{
								this.pingerReference = mdbSystemMailboxPinger;
							}
						}
					}
					if (mdbSystemMailboxPinger == null)
					{
						this.DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult.FailedToGetPinger);
						ExTraceGlobals.DatabasePingerTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[PingerDependentHealthMonitor.Execute] PingerCache returned a null pinger.  Will not ping.  Resource: {0}", base.Key);
					}
					else if (mdbSystemMailboxPinger.LastPingAttemptUtc != DateTime.MinValue && TimeProvider.UtcNow - mdbSystemMailboxPinger.LastPingAttemptUtc < this.PreviousPingInterval && !PingerDependentHealthMonitor.IgnorePingProximity)
					{
						TimeSpan arg = TimeProvider.UtcNow - mdbSystemMailboxPinger.LastPingAttemptUtc;
						this.DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult.PingAttemptTooClose);
						ExTraceGlobals.DatabasePingerTracer.TraceDebug<ResourceKey, TimeSpan, TimeSpan>((long)this.GetHashCode(), "[PingerDependentHealthMonitor.Execute] Will not ping on resource {0} because only {1} has elapsed since the last ping attempt. Expected interval: {2}", base.Key, arg, this.PreviousPingInterval);
					}
					else
					{
						ThreadPool.QueueUserWorkItem(delegate(object state)
						{
							try
							{
								IMdbSystemMailboxPinger mdbSystemMailboxPinger2 = PingerCache.Singleton.Get(this.MdbGuid);
								if (mdbSystemMailboxPinger2 == null)
								{
									this.DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult.FailedToGetPinger);
									ExTraceGlobals.DatabasePingerTracer.TraceDebug<ResourceKey>((long)this.GetHashCode(), "[PingerDependentHealthMonitor.Execute] PingerCache returned a null pinger.  Will not ping.  Resource: {0}", base.Key);
								}
								else if (mdbSystemMailboxPinger2.Ping())
								{
									this.DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult.Pinged);
								}
								else
								{
									Interlocked.Increment(ref this.consecutivePings);
									if (PingerDependentHealthMonitor.OnPingIntervalUpdate != null)
									{
										PingerDependentHealthMonitor.OnPingIntervalUpdate(base.Key, this.consecutivePings, this.CurrentPingInterval);
									}
									this.DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult.FailedPing);
								}
							}
							finally
							{
								lock (this.instanceLock)
								{
									this.pingCheckedOrScheduled = false;
								}
								this.DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult.PingLockReleased);
							}
						});
						flag = false;
					}
				}
			}
			finally
			{
				if (flag)
				{
					lock (this.instanceLock)
					{
						this.pingCheckedOrScheduled = false;
					}
					this.DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult.PingLockReleased);
				}
			}
		}

		internal Guid MdbGuid { get; private set; }

		internal int ConsecutivePings
		{
			get
			{
				return this.consecutivePings;
			}
		}

		protected bool Pinging
		{
			get
			{
				IMdbSystemMailboxPinger mdbSystemMailboxPinger = this.pingerReference;
				return mdbSystemMailboxPinger != null && mdbSystemMailboxPinger.Pinging;
			}
		}

		protected void ReceivedUpdate()
		{
			IMdbSystemMailboxPinger mdbSystemMailboxPinger = this.pingerReference;
			if (mdbSystemMailboxPinger == null && PingerCache.CreatePingerTestHook != null)
			{
				mdbSystemMailboxPinger = PingerCache.CreatePingerTestHook(this.MdbGuid);
			}
			if (mdbSystemMailboxPinger != null)
			{
				if (mdbSystemMailboxPinger.Pinging)
				{
					ExTraceGlobals.DatabasePingerTracer.TraceDebug<Guid>((long)this.GetHashCode(), "RPC traffic due to database pinger for Mdb: {0}", this.MdbGuid);
					Interlocked.Increment(ref this.consecutivePings);
				}
				else
				{
					Interlocked.Exchange(ref this.consecutivePings, 0);
				}
				if (PingerDependentHealthMonitor.OnPingIntervalUpdate != null)
				{
					PingerDependentHealthMonitor.OnPingIntervalUpdate(base.Key, this.consecutivePings, this.CurrentPingInterval);
				}
			}
		}

		private void DoOnExecuteForTest(PingerDependentHealthMonitor.PingResult reason)
		{
			if (PingerDependentHealthMonitor.OnExecuteForTest != null)
			{
				PingerDependentHealthMonitor.OnExecuteForTest(base.Key, this.MdbGuid, reason);
			}
		}

		public static readonly TimeSpan DefaultMinimumPingInterval = TimeSpan.FromSeconds(30.0);

		public static readonly TimeSpan DefaultMaximumPingInterval = TimeSpan.FromSeconds(30.0);

		protected object instanceLock = new object();

		private IMdbSystemMailboxPinger pingerReference;

		private int consecutivePings;

		private bool pingCheckedOrScheduled;

		internal enum PingResult
		{
			None,
			Pinged,
			PingNotNeeded,
			TrafficTooClose,
			PingAttemptTooClose,
			FailedToGetPinger,
			MonitorExpired,
			FailedPing,
			PingTimeNotReached,
			PingAlreadyScheduled,
			PingLockReleased
		}
	}
}
