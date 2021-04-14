using System;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Common.Cache;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class PerTenantCachePerformanceCounters : DefaultCachePerformanceCounters
	{
		public PerTenantCachePerformanceCounters(string cachePerfCounterInstance)
		{
			if (string.IsNullOrEmpty(cachePerfCounterInstance))
			{
				throw new ArgumentNullException("cachePerfCounterInstance");
			}
			try
			{
				this.perfCounters = ConfigurationCachePerfCounters.GetInstance(cachePerfCounterInstance);
			}
			catch (InvalidOperationException ex)
			{
				OwaDiagnostics.Logger.LogEvent(ClientsEventLogConstants.Tuple_ConfigurationCachePerfCountersLoadFailure, string.Empty, new object[]
				{
					cachePerfCounterInstance,
					ex
				});
				this.perfCounters = null;
			}
			if (this.perfCounters != null)
			{
				base.InitializeCounters(this.perfCounters.Requests, this.perfCounters.HitRatio, this.perfCounters.HitRatio_Base, this.perfCounters.CacheSize);
			}
		}

		public override void SizeUpdated(long cacheSize)
		{
			base.SizeUpdated(cacheSize);
			if (this.perfCounters != null)
			{
				this.perfCounters.CacheSizeKB.RawValue = cacheSize / 1024L;
			}
		}

		private const long KB = 1024L;

		private ConfigurationCachePerfCountersInstance perfCounters;
	}
}
