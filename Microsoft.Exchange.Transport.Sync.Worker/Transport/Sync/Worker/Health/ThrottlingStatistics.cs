using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Worker.Throttling;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Transport.Sync.Worker.Health
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ThrottlingStatistics
	{
		internal ThrottlingStatistics()
		{
			this.throttleInfo = new List<ThrottlingInfo>();
		}

		public TimeSpan TotalCpuUnhealthyBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.MailboxCPU, ResourceLoadState.Critical);
			}
		}

		public TimeSpan TotalCpuFairBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.MailboxCPU, ResourceLoadState.Overloaded);
			}
		}

		public TimeSpan TotalCpuUnknownBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.MailboxCPU, ResourceLoadState.Unknown);
			}
		}

		public TimeSpan TotalDatabaseRPCLatencyUnhealthyBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.DatabaseRPCLatency, ResourceLoadState.Critical);
			}
		}

		public TimeSpan TotalDatabaseRPCLatencyFairBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.DatabaseRPCLatency, ResourceLoadState.Overloaded);
			}
		}

		public TimeSpan TotalDatabaseRPCLatencyUnknownBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.DatabaseRPCLatency, ResourceLoadState.Unknown);
			}
		}

		public TimeSpan TotalDatabaseReplicationLogUnhealthyBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.DatabaseReplicationLog, ResourceLoadState.Critical);
			}
		}

		public TimeSpan TotalDatabaseReplicationLogFairBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.DatabaseReplicationLog, ResourceLoadState.Overloaded);
			}
		}

		public TimeSpan TotalDatabaseReplicationLogUnknownBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.DatabaseReplicationLog, ResourceLoadState.Unknown);
			}
		}

		public TimeSpan TotalUnknownUnhealthyBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.Unknown, ResourceLoadState.Critical);
			}
		}

		public TimeSpan TotalUnknownFairBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.Unknown, ResourceLoadState.Overloaded);
			}
		}

		public TimeSpan TotalUnknownUnknownBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime(SyncResourceMonitorType.Unknown, ResourceLoadState.Unknown);
			}
		}

		public TimeSpan TotalBackoffTime
		{
			get
			{
				return this.GetTotalBackOffTime();
			}
		}

		internal void Add(ThrottlingInfo info)
		{
			if (info != null)
			{
				this.throttleInfo.Add(info);
			}
		}

		internal void Update(ThrottlingStatistics stats)
		{
			List<ThrottlingInfo> list = new List<ThrottlingInfo>(stats.throttleInfo);
			lock (this.lockObject)
			{
				if (stats != null && stats.throttleInfo != null)
				{
					foreach (ThrottlingInfo info in list)
					{
						this.Add(info);
					}
				}
			}
		}

		private TimeSpan GetTotalBackOffTime(SyncResourceMonitorType monitor, ResourceLoadState resourceLoadState)
		{
			TimeSpan timeSpan = TimeSpan.Zero;
			foreach (ThrottlingInfo throttlingInfo in this.throttleInfo)
			{
				timeSpan += throttlingInfo.Cache[monitor][resourceLoadState];
			}
			return timeSpan;
		}

		private TimeSpan GetTotalBackOffTime()
		{
			TimeSpan timeSpan = TimeSpan.Zero;
			foreach (ThrottlingInfo throttlingInfo in this.throttleInfo)
			{
				timeSpan += throttlingInfo.BackOffTime;
			}
			return timeSpan;
		}

		private readonly List<ThrottlingInfo> throttleInfo;

		private object lockObject = new object();
	}
}
