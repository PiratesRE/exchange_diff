using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal abstract class CiResourceHealthMonitorBase : CacheableResourceHealthMonitor, IResourceHealthPoller
	{
		protected CiResourceHealthMonitorBase(CiResourceKey key) : base(key)
		{
			this.mdbGuid = key.DatabaseGuid;
			this.average = new FixedTimeAverage((ushort)this.Interval.TotalMilliseconds, 6, Environment.TickCount);
		}

		public bool IsActive
		{
			get
			{
				return true;
			}
		}

		public virtual TimeSpan Interval
		{
			get
			{
				return CiHealthMonitorConfiguration.RefreshInterval;
			}
		}

		protected override int InternalMetricValue
		{
			get
			{
				return (int)Interlocked.Read(ref this.metricValue);
			}
		}

		public virtual void Execute()
		{
			this.metricCalculationStage = "Execute";
			int num = this.CalculateNewMetric();
			if (num >= 0 && num != 2147483647)
			{
				this.lastMetricUpdateUnsuccessfulCount = 0;
				this.average.Add(Environment.TickCount, (uint)num);
				Interlocked.Exchange(ref this.metricValue, (long)((int)this.average.GetValue()));
			}
			else if (this.lastMetricUpdateUnsuccessfulCount < CiHealthMonitorConfiguration.FailedCatalogStatusThreshold)
			{
				this.lastMetricUpdateUnsuccessfulCount++;
			}
			else
			{
				Interlocked.Exchange(ref this.metricValue, (long)num);
			}
			this.LastUpdateUtc = DateTime.UtcNow;
			ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug((long)this.GetHashCode(), "[CiResourceHealthMonitorBase::Execute] (MetricType: {0}, MDB: {1}) Got new metric value of {2}, will report {3}", new object[]
			{
				base.Key.MetricType,
				this.mdbGuid,
				num,
				this.InternalMetricValue
			});
			this.metricCalculationStage = "Completed";
		}

		protected override object GetResourceLoadlInfo(ResourceLoad load)
		{
			return this.info ?? this.metricCalculationStage;
		}

		protected abstract int GetMetricFromStatusInternal(RpcDatabaseCopyStatus2 status);

		private int CalculateNewMetric()
		{
			this.metricCalculationStage = "Calculate";
			if (!CiHealthMonitorConfiguration.Enabled)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceMetricType, Guid>((long)this.GetHashCode(), "[CiResourceHealthMonitorBase::CalculateNewMetric] (MetricType: {0}, MDB: {1}) Disabled, will return metric as unknown.", base.Key.MetricType, this.mdbGuid);
				this.metricCalculationStage = "Disabled";
				return -1;
			}
			if (CiHealthMonitorConfiguration.OverrideMetricValue != null)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceMetricType, Guid, int>((long)this.GetHashCode(), "[CiResourceHealthMonitorBase::CalculateNewMetric] (MetricType: {0}, MDB: {1}) Metric override set to {2}", base.Key.MetricType, this.mdbGuid, CiHealthMonitorConfiguration.OverrideMetricValue.Value);
				this.metricCalculationStage = "Override";
				return CiHealthMonitorConfiguration.OverrideMetricValue.Value;
			}
			List<string> serversForMdb = MdbCopyMonitor.Instance.Value.GetServersForMdb(this.mdbGuid);
			if (serversForMdb == null || serversForMdb.Count == 0)
			{
				ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceMetricType, Guid>((long)this.GetHashCode(), "[CiResourceHealthMonitorBase::CalculateNewMetric] (MetricType: {0}, MDB: {1}) Unable to get topology data from AD, will return metric as unknown.", base.Key.MetricType, this.mdbGuid);
				this.metricCalculationStage = "NoServers";
				return -1;
			}
			List<CiMdbCopyInfo> list = new List<CiMdbCopyInfo>(serversForMdb.Count);
			int num = -1;
			foreach (string text in serversForMdb)
			{
				RpcDatabaseCopyStatus2 rpcDatabaseCopyStatus = MailboxDatabaseCopyStatusCache.Instance.Value.TryGetCopyStatus(text, this.mdbGuid);
				if (rpcDatabaseCopyStatus == null)
				{
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug<ResourceMetricType, Guid, string>((long)this.GetHashCode(), "[CiResourceHealthMonitorBase::CalculateNewMetric] (MetricType: {0}, MDB: {1}) Server {2} failed to report CI status.", base.Key.MetricType, this.mdbGuid, text);
					list.Add(new CiMdbCopyInfo(text));
				}
				else
				{
					int metricFromStatus = this.GetMetricFromStatus(rpcDatabaseCopyStatus.MailboxServer, rpcDatabaseCopyStatus);
					ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug((long)this.GetHashCode(), "[CiResourceHealthMonitorBase::CalculateNewMetric] (MetricType: {0}, MDB: {1}) Server {2} reporting metric as {3}.", new object[]
					{
						base.Key.MetricType,
						this.mdbGuid,
						rpcDatabaseCopyStatus.MailboxServer,
						metricFromStatus
					});
					CiMdbCopyInfo ciMdbCopyInfo = new CiMdbCopyInfo(text, rpcDatabaseCopyStatus.CopyStatus == CopyStatusEnum.Mounted, metricFromStatus);
					if (ciMdbCopyInfo.Mounted)
					{
						num = ciMdbCopyInfo.Metric;
						ciMdbCopyInfo.Used = true;
					}
					list.Add(ciMdbCopyInfo);
				}
			}
			int num2 = Math.Min(CiHealthMonitorConfiguration.NumberOfHealthyCopiesRequired, list.Count);
			list.Sort(delegate(CiMdbCopyInfo copy1, CiMdbCopyInfo copy2)
			{
				if (copy1.Mounted)
				{
					if (copy2.Mounted)
					{
						return 0;
					}
					return -1;
				}
				else
				{
					if (copy2.Mounted)
					{
						return 1;
					}
					if (copy1.Metric < copy2.Metric)
					{
						return -1;
					}
					if (copy1.Metric > copy2.Metric)
					{
						return 1;
					}
					return 0;
				}
			});
			int num3 = -1;
			for (int i = 0; i < num2; i++)
			{
				CiMdbCopyInfo ciMdbCopyInfo2 = list[i];
				ciMdbCopyInfo2.Used = true;
				if (!ciMdbCopyInfo2.Mounted && ciMdbCopyInfo2.Metric > num3)
				{
					num3 = ciMdbCopyInfo2.Metric;
				}
			}
			this.info = new CiMdbInfo(list);
			if (num == -1)
			{
				return -1;
			}
			return Math.Max(num, num3);
		}

		private int GetMetricFromStatus(string serverFqdn, RpcDatabaseCopyStatus2 status)
		{
			int result;
			switch (status.ContentIndexStatus)
			{
			case ContentIndexStatusType.Unknown:
				result = -1;
				break;
			case ContentIndexStatusType.Healthy:
				result = this.GetMetricFromStatusInternal(status);
				break;
			case ContentIndexStatusType.Crawling:
				result = 0;
				break;
			case ContentIndexStatusType.Failed:
			case ContentIndexStatusType.Seeding:
			case ContentIndexStatusType.FailedAndSuspended:
			case ContentIndexStatusType.Suspended:
			case ContentIndexStatusType.AutoSuspended:
				result = int.MaxValue;
				break;
			case ContentIndexStatusType.Disabled:
				result = 0;
				break;
			case ContentIndexStatusType.HealthyAndUpgrading:
				result = 0;
				break;
			default:
				result = -1;
				break;
			}
			ExTraceGlobals.ResourceHealthManagerTracer.TraceDebug((long)this.GetHashCode(), "[CiResourceHealthMonitorBase::GetMetricFromStatus] (MetricType: {0}, MDB: {1}) Server {2} reporting Search status as {3}.", new object[]
			{
				base.Key.MetricType,
				this.mdbGuid,
				serverFqdn,
				status.ContentIndexStatus
			});
			return result;
		}

		private const int Unknown = -1;

		private const int Failed = 2147483647;

		private const int DontThrottle = 0;

		private readonly Guid mdbGuid;

		private readonly FixedTimeAverage average;

		private int lastMetricUpdateUnsuccessfulCount;

		private long metricValue = -1L;

		private CiMdbInfo info;

		private string metricCalculationStage = "NotStarted";
	}
}
