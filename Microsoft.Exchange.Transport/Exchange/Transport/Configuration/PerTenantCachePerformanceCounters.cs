using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.Configuration
{
	internal sealed class PerTenantCachePerformanceCounters : DefaultCachePerformanceCounters
	{
		public PerTenantCachePerformanceCounters(ProcessTransportRole transportRole, string cachePerfCounterInstance)
		{
			if (cachePerfCounterInstance == null)
			{
				throw new ArgumentNullException("cachePerfCounterInstance");
			}
			try
			{
				ConfigurationCachePerfCounters.SetCategoryName(PerTenantCachePerformanceCounters.perfCounterCategoryMap[transportRole]);
				this.perfCounters = ConfigurationCachePerfCounters.GetInstance(cachePerfCounterInstance);
			}
			catch (InvalidOperationException ex)
			{
				ExTraceGlobals.GeneralTracer.TraceError<string, InvalidOperationException>(0L, "Failed to initialize performance counters for component '{0}': {1}", cachePerfCounterInstance, ex);
				ExEventLog exEventLog = new ExEventLog(ExTraceGlobals.GeneralTracer.Category, TransportEventLog.GetEventSource());
				exEventLog.LogEvent(TransportEventLogConstants.Tuple_PerfCountersLoadFailure, null, new object[]
				{
					"Cache",
					cachePerfCounterInstance,
					ex.ToString()
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
				this.perfCounters.CacheSizeMB.RawValue = cacheSize / 1048576L;
			}
		}

		private const long MB = 1048576L;

		private const string EventTag = "Cache";

		private static readonly IDictionary<ProcessTransportRole, string> perfCounterCategoryMap = new Dictionary<ProcessTransportRole, string>
		{
			{
				ProcessTransportRole.Edge,
				"MSExchangeTransport Configuration Cache"
			},
			{
				ProcessTransportRole.Hub,
				"MSExchangeTransport Configuration Cache"
			},
			{
				ProcessTransportRole.FrontEnd,
				"MSExchangeFrontEndTransport Configuration Cache"
			},
			{
				ProcessTransportRole.MailboxDelivery,
				"MSExchange Delivery Configuration Cache"
			},
			{
				ProcessTransportRole.MailboxSubmission,
				"MSExchange Submission Configuration Cache"
			}
		};

		private ConfigurationCachePerfCountersInstance perfCounters;
	}
}
