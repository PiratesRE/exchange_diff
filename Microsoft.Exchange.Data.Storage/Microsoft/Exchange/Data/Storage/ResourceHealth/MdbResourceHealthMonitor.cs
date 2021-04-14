using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MdbResourceHealthMonitor : PingerDependentHealthMonitor
	{
		private static int ReadMinimumRequestRate()
		{
			IntAppSettingsEntry intAppSettingsEntry = new IntAppSettingsEntry("MdbLatencyMonitor.MinimumRequestRate", 20, ExTraceGlobals.ResourceHealthManagerTracer);
			if (intAppSettingsEntry.Value < 0)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceError<int>(0L, "[MdbResourceHealthMonitor.ReadMinimumRequestRate] App.Config request rate is invalid '{0}'.  Must be >= 0", intAppSettingsEntry.Value);
				return 20;
			}
			return intAppSettingsEntry.Value;
		}

		internal static int MinimumAcceptableRequestsPerSecond
		{
			get
			{
				return MdbResourceHealthMonitor.minimumAcceptableRequestsPerSecond;
			}
		}

		internal static int SetMinimumRequestRateForTest(int newRate)
		{
			int result = MdbResourceHealthMonitor.minimumAcceptableRequestsPerSecond;
			MdbResourceHealthMonitor.minimumAcceptableRequestsPerSecond = newRate;
			return result;
		}

		internal MdbResourceHealthMonitor(MdbResourceHealthMonitorKey key) : base(key, key.DatabaseGuid)
		{
			this.rpcLatencyAverage = new FixedTimeAverage(1000, 60, Environment.TickCount);
		}

		internal void Reset()
		{
			lock (this.instanceLock)
			{
				int tickCount = Environment.TickCount;
				this.rpcLatencyAverage.Clear(tickCount);
			}
		}

		private void Update()
		{
			lock (this.instanceLock)
			{
				this.rpcLatencyAverage.Update(Environment.TickCount);
			}
		}

		public void Update(int averageRpcLatency, uint operationCount)
		{
			lock (this.instanceLock)
			{
				DateTime utcNow = TimeProvider.UtcNow;
				TimeSpan t = utcNow - this.lastOperationUpdateUtc;
				if (!(t < MdbResourceHealthMonitor.UpdateInterval))
				{
					if (this.lastOperationUpdateUtc == DateTime.MinValue || operationCount < this.oldOperationCount)
					{
						this.oldOperationCount = operationCount;
						this.lastOperationUpdateUtc = utcNow;
					}
					else
					{
						uint num = (uint)((operationCount - this.oldOperationCount) / t.TotalSeconds);
						this.oldOperationCount = operationCount;
						this.lastOperationUpdateUtc = utcNow;
						if ((ulong)num >= (ulong)((long)MdbResourceHealthMonitor.MinimumAcceptableRequestsPerSecond) || base.Pinging)
						{
							this.Update(averageRpcLatency);
						}
					}
				}
			}
		}

		internal void Update(int averageRpcLatency)
		{
			DateTime utcNow = TimeProvider.UtcNow;
			TimeSpan t = utcNow - this.RawLastUpdateUtc;
			if (t < MdbResourceHealthMonitor.UpdateInterval)
			{
				return;
			}
			base.ReceivedUpdate();
			this.LastUpdateUtc = utcNow;
			this.lastRPCLatencyValue = averageRpcLatency;
			this.rpcLatencyAverage.Add((uint)averageRpcLatency);
			base.FireNotifications();
		}

		public override ResourceHealthMonitorWrapper CreateWrapper()
		{
			return new MdbResourcehealthMonitorWrapper(this);
		}

		public int LastRPCLatencyValue
		{
			get
			{
				return this.lastRPCLatencyValue;
			}
		}

		public int AverageRPCLatencyValue
		{
			get
			{
				int result;
				lock (this.instanceLock)
				{
					this.Update();
					result = (int)this.rpcLatencyAverage.GetValue();
				}
				return result;
			}
		}

		public override DateTime LastUpdateUtc
		{
			get
			{
				if (this.AverageUtcUpdateNeeded(base.LastUpdateUtc))
				{
					lock (this.lastAverageUpdateUtcLock)
					{
						DateTime lastUpdateUtc = base.LastUpdateUtc;
						if (this.AverageUtcUpdateNeeded(lastUpdateUtc))
						{
							this.lastAverageUpdateUtc = new DateTime?(lastUpdateUtc);
						}
					}
				}
				return this.lastAverageUpdateUtc.Value;
			}
		}

		internal override DateTime RawLastUpdateUtc
		{
			get
			{
				return base.LastUpdateUtc;
			}
		}

		protected override int InternalMetricValue
		{
			get
			{
				if (!this.rpcLatencyAverage.IsEmpty)
				{
					return this.AverageRPCLatencyValue;
				}
				return -1;
			}
		}

		private bool AverageUtcUpdateNeeded(DateTime lastUpdateUtc)
		{
			return this.lastAverageUpdateUtc == null || (this.lastAverageUpdateUtc.Value != lastUpdateUtc && TimeProvider.UtcNow - this.lastAverageUpdateUtc.Value > TimeSpan.FromSeconds(60.0));
		}

		private const int TimeWindowInSeconds = 60;

		private const string MinimumRequestRateKeyName = "MdbLatencyMonitor.MinimumRequestRate";

		private const int DefaultMinimumRequestRate = 20;

		private const int MsecInOneSecond = 1000;

		private static int minimumAcceptableRequestsPerSecond = MdbResourceHealthMonitor.ReadMinimumRequestRate();

		private DateTime? lastAverageUpdateUtc = null;

		private object lastAverageUpdateUtcLock = new object();

		private DateTime lastOperationUpdateUtc = DateTime.MinValue;

		private uint oldOperationCount;

		public static TimeSpan UpdateInterval = TimeSpan.FromSeconds(1.0);

		private FixedTimeAverage rpcLatencyAverage;

		private volatile int lastRPCLatencyValue;
	}
}
